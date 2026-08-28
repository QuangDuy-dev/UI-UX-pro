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
            var max = Math.Clamp(maxItems ?? admin.MaxItemsPerRun, 1, 20);

            // Tải toàn bộ item hiện có 1 lần để chống trùng: theo tên chuẩn hoá, theo content hash,
            // và đếm số lần mỗi template đã được dùng (giới hạn số biến thể).
            var existing = await _animations.GetAllAsync(includeNonPublic: true);
            var existingNames = new HashSet<string>(
                existing.Select(x => AnimationService.NormalizeName(x.Name)), StringComparer.Ordinal);
            var existingHashes = new HashSet<string>(
                existing.Select(x => x.ContentHash).Where(h => !string.IsNullOrWhiteSpace(h)).Select(h => h!), StringComparer.Ordinal);
            var templateUsage = BuildTemplateUsage(existing);

            // Keywords đa dạng: giữ tối đa 2 keyword MỚI từ agent, phần còn lại lấy từ kho phong cách
            // (DesignStyleLibrary) xoay vòng theo ngày, loại trừ style đã dùng trong 30 ngày gần nhất.
            var recentKeywords = await GetRecentKeywordsAsync(days: 30);
            var selectedKeywords = ResolveKeywords(keywords, admin.TrendKeywords, defaults, recentKeywords, max);
            report.Keywords = selectedKeywords;

            // Bật AI chỉ khi user bật ở Settings (admin.AiEnabled).
            // Nếu LLM gọi lỗi (401 key sai, 429, network...) thì tự fallback template
            // để job luôn sinh được animation, không bao giờ lộ lỗi 401 từ provider.
            var skipped = new List<string>();
            var created = new List<string>();
            string? llmError = null;
            var seenRunNames = new HashSet<string>(StringComparer.Ordinal);
            var seenRunHashes = new HashSet<string>(StringComparer.Ordinal);

            // 1) Sinh bằng LLM (nếu bật AI)
            if (admin.AiEnabled)
            {
                var (llmItems, llmErr) = await TryGenerateWithLlmAsync(selectedKeywords, max);
                llmError = llmErr;
                await InsertUniqueAsync(llmItems, existingNames, existingHashes, seenRunNames, seenRunHashes, skipped, created);
            }

            // 2) BÙ VÀO nếu chưa đủ max: item trùng bị chặn thì tự tìm template/palette khác
            //    để luôn đạt đủ số lượng (LLM sinh ít/trùng, hoặc AI tắt -> template fill tới max).
            if (created.Count < max)
            {
                var remaining = max - created.Count;
                var tplItems = GenerateFromTemplates(selectedKeywords, remaining, existingNames, existingHashes, templateUsage, skipped);
                await InsertUniqueAsync(tplItems, existingNames, existingHashes, seenRunNames, seenRunHashes, skipped, created);
            }

            report.GeneratedItemIds = created;
            report.SkippedCount = skipped.Count;
            report.SkippedReasons = skipped;
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

    /// <summary>
    /// Lưu item với dedup THẬT: trùng tên chuẩn hoá HOẶC trùng nội dung (hash) -> BỎ QUA
    /// (không đổi slug thành -2, -3 rồi lưu tiếp như trước đây). Tên/hash của item đã lưu
    /// được thêm vào tập existing để các lần sinh sau trong cùng run không tạo lại.
    /// </summary>
    private async Task InsertUniqueAsync(
        List<AnimationItem> items,
        HashSet<string> existingNames,
        HashSet<string> existingHashes,
        HashSet<string> seenRunNames,
        HashSet<string> seenRunHashes,
        List<string> skipped,
        List<string> created)
    {
        foreach (var item in items)
        {
            var norm = AnimationService.NormalizeName(item.Name);
            var hash = AnimationService.ComputeContentHash(item.Html, item.Css, item.Js);
            if (!seenRunNames.Add(norm) || !seenRunHashes.Add(hash) ||
                existingNames.Contains(norm) || existingHashes.Contains(hash))
            {
                skipped.Add($"Trùng: {item.Name}");
                continue;
            }

            existingNames.Add(norm);
            existingHashes.Add(hash);
            item.Slug = await _animations.EnsureUniqueSlugAsync(item.Slug);
            await _animations.CreateAsync(item);
            created.Add(item.Id.ToString());
        }
    }

    public async Task<List<TrendReport>> GetRecentReportsAsync(int limit = 10)
    {
        return await _db.TrendReports.Find(_ => true).SortByDescending(r => r.RunDate).Limit(limit).ToListAsync();
    }

    /// <summary>Gom các keyword/style đã dùng trong N ngày gần nhất (từ lịch sử trend reports).</summary>
    private async Task<HashSet<string>> GetRecentKeywordsAsync(int days)
    {
        var from = DateTime.UtcNow.AddDays(-days);
        var reports = await _db.TrendReports.Find(r => r.RunDate >= from).ToListAsync();
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in reports)
        foreach (var k in r.Keywords)
            if (!string.IsNullOrWhiteSpace(k))
                set.Add(k.Trim().ToLowerInvariant());
        return set;
    }

    /// <summary>
    /// Chọn keywords cho lần chạy: tối đa 2 keyword MỚI LẠ từ agent, lấp đầy bằng style chưa dùng
    /// 30 ngày từ DesignStyleLibrary (xoay vòng theo ngày để mỗi ngày ra phong cách khác nhau).
    /// </summary>
    private static List<string> ResolveKeywords(
        List<string>? agentKeywords,
        List<string> adminKeywords,
        List<string> defaults,
        HashSet<string> recent,
        int max)
    {
        var result = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void AddFresh(string? k)
        {
            k = k?.Trim();
            if (string.IsNullOrWhiteSpace(k) || !seen.Add(k)) return;
            if (!recent.Contains(k)) result.Add(k);
        }

        // 1) Tối đa 2 keyword mới lạ từ agent (không trùng style đã dùng 30 ngày)
        foreach (var k in agentKeywords ?? new List<string>())
        {
            if (result.Count >= Math.Min(2, max)) break;
            AddFresh(k);
        }

        // 2) Lấp đầy từ kho phong cách xoay vòng theo ngày
        var pool = DesignStyleLibrary.PickRotation(recent, Math.Max(max, 8), DateTime.UtcNow.DayOfYear);
        foreach (var k in pool)
        {
            if (result.Count >= max) break;
            AddFresh(k);
        }

        // 3) Còn thiếu: cho phép dùng lại keyword (agent/admin/default) để đủ số lượng
        foreach (var k in (agentKeywords ?? new List<string>()).Concat(adminKeywords).Concat(defaults))
        {
            if (result.Count >= max) break;
            var key = k?.Trim();
            if (!string.IsNullOrWhiteSpace(key) && seen.Add(key)) result.Add(key);
        }

        // 4) Hiếm khi vẫn rỗng: dùng thẳng pool (cho phép trùng)
        if (result.Count == 0)
        {
            foreach (var k in pool)
            {
                if (result.Count >= max) break;
                if (seen.Add(k)) result.Add(k);
            }
        }

        return result.Take(max).ToList();
    }

    /// <summary>Đếm số lần mỗi template (theo tên gốc trước dấu " — ") đã xuất hiện trong DB.</summary>
    private static Dictionary<string, int> BuildTemplateUsage(List<AnimationItem> existing)
    {
        var usage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var x in existing)
        {
            var baseName = GetTemplateBaseName(x.Name);
            usage[baseName] = usage.GetValueOrDefault(baseName) + 1;
        }
        return usage;
    }

    private static string GetTemplateBaseName(string name)
    {
        foreach (var sep in new[] { " — ", " – ", " - " })
        {
            var idx = name.IndexOf(sep, StringComparison.Ordinal);
            if (idx > 0) return name[..idx].Trim();
        }
        return name.Trim();
    }

    private List<AnimationItem> GenerateFromTemplates(
        List<string> keywords,
        int max,
        HashSet<string>? existingNames = null,
        HashSet<string>? existingHashes = null,
        Dictionary<string, int>? templateUsage = null,
        List<string>? skipped = null)
    {
        if (max <= 0) return new List<AnimationItem>();

        var templates = TemplateLibrary.All.ToList();
        var keywordList = keywords.Count > 0 ? keywords : new List<string> { "animation" };

        // 1) Thứ tự ưu tiên template: khớp keyword -> chưa dùng (trải đều category) -> còn lại
        var ordered = new List<AnimationTemplate>();

        // 1a) Khớp keyword với template (name/tags/category chứa keyword)
        foreach (var kw in keywordList)
        {
            var k = kw.ToLowerInvariant();
            var match = templates.FirstOrDefault(t =>
                t.Name.ToLowerInvariant().Contains(k) ||
                t.Tags.Any(tag => tag.ToLowerInvariant().Contains(k)) ||
                t.CategorySlug.Contains(k) ||
                t.CategoryName.ToLowerInvariant().Contains(k));
            if (match is not null)
            {
                ordered.Add(match);
                templates.Remove(match);
            }
        }

        // 1b) Template chưa từng dùng (usage == 0), trải đều category round-robin
        var unused = templates.Where(t => templateUsage is null || templateUsage.GetValueOrDefault(t.Name) == 0).ToList();
        var byCategory = unused.GroupBy(t => t.CategorySlug).ToList();
        var longest = byCategory.Count == 0 ? 0 : byCategory.Max(g => g.Count());
        for (int i = 0; i < longest; i++)
        {
            foreach (var g in byCategory)
            {
                if (i < g.Count())
                {
                    ordered.Add(g.ElementAt(i));
                    templates.Remove(g.ElementAt(i));
                }
            }
        }

        // 1c) Các template còn lại (đã dùng -> chỉ tạo biến thể màu mới nếu palette chưa trùng)
        ordered.AddRange(templates);

        if (ordered.Count == 0) ordered = TemplateLibrary.All.ToList();

        var palettes = new[]
        {
            (Name: "Original", C1: "#22d3ee", C2: "#6366f1", C3: "#ec4899"),
            (Name: "Sunset", C1: "#fbbf24", C2: "#f97316", C3: "#ef4444"),
            (Name: "Mint", C1: "#34d399", C2: "#14b8a6", C3: "#0ea5e9"),
            (Name: "Berry", C1: "#f472b6", C2: "#8b5cf6", C3: "#3b82f6"),
            (Name: "Grape", C1: "#a78bfa", C2: "#ec4899", C3: "#f43f5e"),
            (Name: "Sky", C1: "#38bdf8", C2: "#818cf8", C3: "#c084fc"),
            (Name: "Flame", C1: "#fde047", C2: "#f59e0b", C3: "#dc2626"),
            (Name: "Ocean", C1: "#6ee7b7", C2: "#2dd4bf", C3: "#6366f1")
        };
        var paletteStart = Math.Abs(DateTime.UtcNow.DayOfYear) % palettes.Length;

        var items = new List<AnimationItem>();
        var seenNames = new HashSet<string>(StringComparer.Ordinal);
        var seenHashes = new HashSet<string>(StringComparer.Ordinal);

        // 2) Duyệt template theo thứ tự; với mỗi template thử dần 8 palette cho tới khi
        //    tìm được tổ hợp KHÔNG trùng (tên + nội dung) — trùng thì bù bằng tổ hợp khác
        //    hoặc template khác, đảm bảo luôn đủ max item mới.
        for (int ti = 0; ti < ordered.Count && items.Count < max; ti++)
        {
            var tpl = ordered[ti];
            var keyword = keywordList[items.Count % keywordList.Count];
            var added = false;

            for (int pi = 0; pi < palettes.Length && !added && items.Count < max; pi++)
            {
                var palette = pi == 0
                    ? palettes[0]
                    : palettes[1 + ((paletteStart + pi - 1) % (palettes.Length - 1))];
                var name = pi == 0
                    ? $"{tpl.Name} — {ToTitle(keyword)}"
                    : $"{tpl.Name} — {ToTitle(keyword)} · {palette.Name}";
                var norm = AnimationService.NormalizeName(name);
                if (seenNames.Contains(norm) || (existingNames?.Contains(norm) ?? false)) continue;

                var css = tpl.Css
                    .Replace("#22d3ee", palette.C1)
                    .Replace("#6366f1", palette.C2)
                    .Replace("#ec4899", palette.C3);
                var hash = AnimationService.ComputeContentHash(tpl.Html, css, tpl.Js);
                if (seenHashes.Contains(hash) || (existingHashes?.Contains(hash) ?? false)) continue;

                seenNames.Add(norm);
                seenHashes.Add(hash);
                items.Add(new AnimationItem
                {
                    Name = name,
                    Slug = AnimationService.Slugify(name),
                    Description = tpl.Description,
                    CategorySlug = tpl.CategorySlug,
                    CategoryName = tpl.CategoryName,
                    Tags = tpl.Tags.Concat(new[] { keyword.ToLowerInvariant(), palette.Name.ToLowerInvariant() }).Distinct().ToList(),
                    Html = tpl.Html,
                    Css = css,
                    Js = tpl.Js,
                    Source = AnimationSource.Daily,
                    Status = ItemStatus.Published,
                    IsPublic = true,
                    TrendKeywords = new List<string>(keywords)
                });
                added = true;
            }

            if (!added) skipped?.Add($"Hết biến thể mới: {tpl.Name}");
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

        // Chống trùng: LLM biết style nào đã dùng gần đây và component nào đã tồn tại trên web.
        var recentKeywords = await GetRecentKeywordsAsync(days: 30);
        var recentNames = await _animations.GetRecentNamesAsync(limit: 40);
        var avoidStyles = string.Join(", ", recentKeywords.Take(25));
        var avoidNames = string.Join("\n- ", recentNames.Take(40));

        var prompt =
            "You are a senior UI/UX developer. Generate exactly " + max +
            " BRAND-NEW, visually DISTINCT animated components — ONE component per style keyword below.\n" +
            "Today's target styles:\n- " + string.Join("\n- ", keywords) + "\n" +
            "\n" +
            "Styles ALREADY heavily used on this site recently (do NOT use them as the primary style):\n" +
            avoidStyles + "\n" +
            "\n" +
            "The site ALREADY has components with these names (do NOT recreate or rename anything similar):\n- " +
            avoidNames + "\n" +
            "\n" +
            "Rules:\n" +
            "- Each component must use a DIFFERENT category AND a DIFFERENT visual style — no two items may look alike.\n" +
            "- Combine today's target style with a fresh idea: new layout, new interaction, new color mood.\n" +
            "- Each component must be a SINGLE self-contained snippet (no frameworks).\n" +
            "- Output STRICT JSON only, no markdown, no explanation, exactly this shape:\n" +
            "{\n" +
            "  \"items\": [\n" +
            "    {\n" +
            "      \"name\": \"Component name (unique, descriptive, NOT similar to the existing list above)\",\n" +
            "      \"category\": \"one of: nav, hero, button, card, list, table, form, loader, modal, toast, tabs, accordion, badge, progress, carousel, marquee, counter, chat, footer, dropdown, pricing, scroll\",\n" +
            "      \"description\": \"Short description\",\n" +
            "      \"tags\": [\"<its target style keyword>\",\"tag2\",\"tag3\"],\n" +
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
            temperature = 0.9,
            response_format = new { type = "json_object" },
            messages = new[]
            {
                new { role = "system", content = "You generate production-ready animated UI components as strict JSON." },
                new { role = "user", content = prompt }
            }
        };

        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(4) };
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
        return items.Take(max).ToList();
    }

    private static string ToTitle(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(' ', parts.Select(p => char.ToUpper(p[0]) + p[1..]));
    }
}
