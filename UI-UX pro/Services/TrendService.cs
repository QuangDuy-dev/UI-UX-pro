using System.Text;
using System.Text.Json;
using MongoDB.Driver;
using UI_UX_pro.Data;
using UI_UX_pro.Models;

namespace UI_UX_pro.Services;

public class TrendService
{
    private readonly AnimationService _animations;
    private readonly MongoDbContext _db;
    private readonly IConfiguration _config;
    private readonly AdminConfigService _adminConfig;
    private readonly ILogger<TrendService> _logger;

    public TrendService(AnimationService animations, MongoDbContext db, IConfiguration config, AdminConfigService adminConfig, ILogger<TrendService> logger)
    {
        _animations = animations;
        _db = db;
        _config = config;
        _logger = logger;
        _adminConfig = adminConfig;
    }

    /// <summary>
    /// Sinh animation mới từ danh sách keywords trend. Nếu AI được bật (cấu hình trong
    /// admin-config.json) thì dùng LLM sinh code; ngược lại dùng template library.
    /// </summary>
    public async Task<TrendReport> GenerateDailyAsync(
        List<string>? keywords = null,
        string? summary = null,
        List<string>? sourceUrls = null,
        int? maxItems = null)
    {
        var report = new TrendReport
        {
            RunDate = DateTime.UtcNow,
            Keywords = keywords ?? new List<string>(),
            Summary = summary,
            SourceUrls = sourceUrls ?? new List<string>()
        };

        try
        {
            // Cấu hình AI/automation lấy từ file admin-config.json (nguồn sự thật sau khi user đổi ở Settings).
            var admin = _adminConfig.Load(_config);

            var defaults = _config.GetSection("DailyTrend:DefaultKeywords").Get<List<string>>() ?? new List<string>();
            if (report.Keywords.Count == 0)
                report.Keywords = admin.TrendKeywords.Count > 0 ? admin.TrendKeywords : defaults;

            var max = maxItems ?? admin.MaxItemsPerRun;
            max = Math.Clamp(max, 1, 20);

            // Bật AI chỉ khi user bật ở Settings (admin.AiEnabled).
            // Nếu LLM gọi lỗi (401 API key sai, 429, network...) thì tự fallback template
            // để job luôn sinh được animation, không bao giờ lộ lỗi 401 từ provider.
            var (items, llmError) = admin.AiEnabled
                ? await TryGenerateWithLlmAsync(report.Keywords, max)
                : (GenerateFromTemplates(report.Keywords, max), (string?)null);

            var created = new List<string>();
            foreach (var item in items)
            {
                if (await _animations.SlugExistsAsync(item.Slug))
                    item.Slug = await _animations.EnsureUniqueSlugAsync(item.Slug);
                await _animations.CreateAsync(item);
                created.Add(item.Id.ToString());
            }

            report.GeneratedItemIds = created;
            report.Status = created.Count > 0
                ? (string.IsNullOrWhiteSpace(llmError) ? "success" : "partial")
                : "empty";
            if (!string.IsNullOrWhiteSpace(llmError))
                report.Error = llmError;

            await _db.TrendReports.InsertOneAsync(report);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Daily trend generation failed");
            report.Status = "error";
            report.Error = ex.Message;
            await _db.TrendReports.InsertOneAsync(report);
            return report;
        }
    }

    public async Task<List<TrendReport>> GetRecentReportsAsync(int limit = 10)
    {
        return await _db.TrendReports.Find(_ => true).SortByDescending(r => r.RunDate).Limit(limit).ToListAsync();
    }

    private List<AnimationItem> GenerateFromTemplates(List<string> keywords, int max)
    {
        var templates = TemplateLibrary.All.ToList();
        var selected = new List<AnimationTemplate>();

        foreach (var kw in keywords)
        {
            if (selected.Count >= max) break;
            var k = kw.ToLowerInvariant();
            var match = templates.FirstOrDefault(t =>
                t.Name.ToLowerInvariant().Contains(k) ||
                t.Tags.Any(tag => tag.ToLowerInvariant().Contains(k)) ||
                k.Contains(t.CategorySlug) ||
                t.CategoryName.ToLowerInvariant().Contains(k));

            if (match is not null)
            {
                selected.Add(match);
                templates.Remove(match);
            }
        }

        if (selected.Count < max)
        {
            foreach (var t in templates.Take(max - selected.Count))
                selected.Add(t);
        }

        if (selected.Count == 0)
            selected = TemplateLibrary.All.Take(max).ToList();

        var palettes = new[]
        {
            ("#22d3ee", "#6366f1", "#ec4899"),
            ("#f472b6", "#8b5cf6", "#3b82f6"),
            ("#34d399", "#14b8a6", "#0ea5e9"),
            ("#fbbf24", "#f97316", "#ef4444")
        };

        var items = new List<AnimationItem>();
        var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (tpl, i) in selected.Select((t, i) => (t, i)))
        {
            var keyword = keywords.Count > 0 ? keywords[i % keywords.Count] : tpl.Tags[0];
            var name = $"{tpl.Name} — {ToTitle(keyword)}";
            if (!usedNames.Add(name)) continue;

            var (c1, c2, c3) = palettes[i % palettes.Length];
            var css = tpl.Css
                .Replace("#22d3ee", c1)
                .Replace("#6366f1", c2)
                .Replace("#ec4899", c3);

            var item = new AnimationItem
            {
                Name = name,
                Slug = AnimationService.Slugify(name),
                Description = tpl.Description,
                CategorySlug = tpl.CategorySlug,
                CategoryName = tpl.CategoryName,
                Tags = tpl.Tags.Concat(new[] { keyword.ToLowerInvariant() }).Distinct().ToList(),
                Html = tpl.Html,
                Css = css,
                Js = tpl.Js,
                Source = AnimationSource.Daily,
                Status = ItemStatus.Published,
                IsPublic = true,
                TrendKeywords = new List<string>(keywords)
            };
            items.Add(item);
        }

        return items;
    }

    /// <summary>
    /// Gọi LLM sinh animation; nếu LLM lỗi (401 key sai, 429, network, model không hợp lệ...)
    /// thì tự fallback về template-based để job luôn tạo được component mới.
    /// </summary>
    private async Task<(List<AnimationItem> Items, string? Error)> TryGenerateWithLlmAsync(
        List<string> keywords, int max)
    {
        try
        {
            var items = await GenerateWithLlmAsync(keywords, max);
            return (items, null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "LLM generation failed ({Message}) — falling back to templates.", ex.Message);
            return (GenerateFromTemplates(keywords, max), "LLM failed: " + ex.Message);
        }
    }

    private async Task<List<AnimationItem>> GenerateWithLlmAsync(List<string> keywords, int max)
    {
        var admin = _adminConfig.Load(_config);
        var apiKey = admin.AiApiKey;
        var endpoint = string.IsNullOrWhiteSpace(admin.AiEndpoint)
            ? "https://api.openai.com/v1/chat/completions"
            : admin.AiEndpoint;
        var model = string.IsNullOrWhiteSpace(admin.AiModel) ? "gpt-4o-mini" : admin.AiModel;

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Ai enabled but no API key configured — falling back to templates.");
            return GenerateFromTemplates(keywords, max);
        }

        var prompt =
            "You are a senior UI/UX developer. Generate " + max +
            " fresh, original, copy-paste-ready HTML/CSS/vanilla-JS animated components " +
            "inspired by these current UI/UX trends: " + string.Join(", ", keywords) + "\n" +
            "\n" +
            "Rules:\n" +
            "- Each component must be a SINGLE self-contained snippet (no frameworks).\n" +
            "- Output STRICT JSON only, no markdown, no explanation, exactly this shape:\n" +
            "{\n" +
            "  \"items\": [\n" +
            "    {\n" +
            "      \"name\": \"Component name\",\n" +
            "      \"category\": \"one of: nav, hero, button, card, list, table, form, loader, modal, toast, tabs, accordion, badge, progress, carousel, marquee, counter, chat, footer, dropdown, pricing, scroll\",\n" +
            "      \"description\": \"Short description\",\n" +
            "      \"tags\": [\"tag1\",\"tag2\"],\n" +
            "      \"html\": \"<fragment html only>\",\n" +
            "      \"css\": \"raw css only, no <style> tag\",\n" +
            "      \"js\": \"raw js only, no <script> tag, can be empty string\"\n" +
            "    }\n" +
            "  ]\n" +
            "}\n" +
            "- CSS must be scoped with unique class names (prefix \"tnd-\").\n" +
            "- The html/css/js must work inside a sandboxed iframe with no external resources.";

        var body = new
        {
            model,
            temperature = 0.8,
            response_format = new { type = "json_object" },
            messages = new[]
            {
                new { role = "system", content = "You generate production-ready animated UI components as strict JSON." },
                new { role = "user", content = prompt }
            }
        };

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        var json = JsonSerializer.Serialize(body);
        var resp = await client.PostAsync(endpoint, new StringContent(json, Encoding.UTF8, "application/json"));
        resp.EnsureSuccessStatusCode();

        var raw = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(raw);
        var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "{}";

        using var itemsDoc = JsonDocument.Parse(content);
        var items = new List<AnimationItem>();
        if (itemsDoc.RootElement.TryGetProperty("items", out var arr))
        {
            foreach (var el in arr.EnumerateArray())
            {
                var name = el.TryGetProperty("name", out var n) ? n.GetString() ?? "Untitled" : "Untitled";
                var catSlug = el.TryGetProperty("category", out var c) ? (c.GetString() ?? "misc").ToLowerInvariant() : "misc";
                var item = new AnimationItem
                {
                    Name = name,
                    Slug = AnimationService.Slugify(name),
                    Description = el.TryGetProperty("description", out var d) ? d.GetString() : null,
                    CategorySlug = catSlug,
                    CategoryName = TemplateLibrary.CategoryNames.TryGetValue(catSlug, out var cn) ? cn : "Misc",
                    Tags = el.TryGetProperty("tags", out var tg) ? tg.EnumerateArray().Select(x => x.GetString() ?? "").Where(s => s.Length > 0).ToList() : new List<string>(),
                    Html = el.TryGetProperty("html", out var h) ? h.GetString() ?? "" : "",
                    Css = el.TryGetProperty("css", out var cs) ? cs.GetString() ?? "" : "",
                    Js = el.TryGetProperty("js", out var js) ? js.GetString() ?? "" : "",
                    Source = AnimationSource.Ai,
                    Status = ItemStatus.Published,
                    IsPublic = true,
                    TrendKeywords = new List<string>(keywords)
                };
                if (!string.IsNullOrWhiteSpace(item.Html)) items.Add(item);
            }
        }
        return items;
    }

    private static string ToTitle(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(' ', parts.Select(p => char.ToUpper(p[0]) + p[1..]));
    }
}
