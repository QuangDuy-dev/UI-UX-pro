using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using UI_UX_pro.Filters;
using UI_UX_pro.Models;
using UI_UX_pro.Services;

namespace UI_UX_pro.Controllers;

[ApiController]
[Route("api")]
[AllowAnonymous]   // API không nằm trong vùng Admin auth; chỉ bảo vệ bởi [AgentApiKey] (nếu có cấu hình)
[AgentApiKey]
public class ApiController : ControllerBase
{
    private readonly AnimationService _animations;
    private readonly TrendService _trends;

    public ApiController(AnimationService animations, TrendService trends)
    {
        _animations = animations;
        _trends = trends;
    }

    public class DailyTrendRequest
    {
        public List<string>? Keywords { get; set; }
        public string? Summary { get; set; }
        public List<string>? SourceUrls { get; set; }
        public int? MaxItems { get; set; }
    }

    public class CreateItemRequest
    {
        public string Name { get; set; } = "";
        public string? CategorySlug { get; set; }
        public string? Description { get; set; }
        public List<string>? Tags { get; set; }
        public string Html { get; set; } = "";
        public string? Css { get; set; }
        public string? Js { get; set; }
    }

    /// <summary>Danh sách animation công khai (dùng để agent kiểm tra những gì đã có).</summary>
    [HttpGet("animations")]
    public async Task<IActionResult> List([FromQuery] string? category = null, [FromQuery] string? q = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var (items, total) = await _animations.SearchAsync(category, q, "newest", page, Math.Clamp(pageSize, 1, 100));
        return Ok(new
        {
            total,
            page,
            items = items.Select(x => new
            {
                id = x.Id.ToString(),
                x.Slug,
                x.Name,
                x.Description,
                x.CategorySlug,
                x.CategoryName,
                x.Tags,
                x.Source,
                x.ViewCount,
                x.LikeCount,
                x.CreatedAt
            })
        });
    }

    /// <summary>Endpoint chính: cron/agent gọi mỗi ngày để sinh animation mới từ trends.</summary>
    [HttpPost("daily-trends/run")]
    public async Task<IActionResult> RunDaily([FromBody] DailyTrendRequest? req)
    {
        var report = await _trends.GenerateDailyAsync(
            req?.Keywords,
            req?.Summary,
            req?.SourceUrls,
            req?.MaxItems);

        return Ok(new
        {
            status = report.Status,
            runDate = report.RunDate,
            keywords = report.Keywords,
            summary = report.Summary,
            sourceUrls = report.SourceUrls,
            error = report.Error,
            createdCount = report.GeneratedItemIds.Count,
            createdIds = report.GeneratedItemIds,
            skippedCount = report.SkippedCount,
            skippedReasons = report.SkippedReasons,
            reportId = report.Id.ToString()
        });
    }

    /// <summary>Lịch sử các lần chạy daily.</summary>
    [HttpGet("daily-trends/reports")]
    public async Task<IActionResult> Reports([FromQuery] int limit = 10)
    {
        var reports = await _trends.GetRecentReportsAsync(Math.Clamp(limit, 1, 50));
        return Ok(reports.Select(r => new
        {
            id = r.Id.ToString(),
            r.RunDate,
            r.Keywords,
            r.Summary,
            r.SourceUrls,
            r.Status,
            r.Error,
            createdCount = r.GeneratedItemIds.Count,
            skippedCount = r.SkippedCount
        }));
    }

    /// <summary>Agent dùng để upload một animation cụ thể (không qua form web).</summary>
    [HttpPost("animations")]
    public async Task<IActionResult> Create([FromBody] CreateItemRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(req.Html))
            return BadRequest(new { error = "Name and Html are required." });

        var catSlug = (req.CategorySlug ?? "misc").ToLowerInvariant();
        var catName = TemplateLibrary.CategoryNames.TryGetValue(catSlug, out var cn) ? cn : "Misc";
        if (TemplateLibrary.CategoryNames.ContainsKey(catSlug) == false && catSlug != "misc")
            catName = "Misc";

        var item = new AnimationItem
        {
            Name = req.Name.Trim(),
            Slug = await _animations.EnsureUniqueSlugAsync(AnimationService.Slugify(req.Name)),
            Description = req.Description,
            CategorySlug = catSlug,
            CategoryName = catName,
            Tags = req.Tags ?? new List<string>(),
            Html = req.Html,
            Css = req.Css ?? "",
            Js = req.Js ?? "",
            Source = AnimationSource.Ai,
            Status = ItemStatus.Published,
            IsPublic = true
        };

        await _animations.CreateAsync(item);
        return Ok(new { id = item.Id.ToString(), item.Slug, item.Name, status = "created" });
    }

    /// <summary>Tăng lượt xem.</summary>
    [HttpPost("animations/{id}/view")]
    public async Task<IActionResult> View(string id)
    {
        await _animations.IncrementViewAsync(id);
        return Ok(new { ok = true });
    }

    /// <summary>Tăng lượt like.</summary>
    [HttpPost("animations/{id}/like")]
    public async Task<IActionResult> Like(string id)
    {
        await _animations.IncrementLikeAsync(id);
        return Ok(new { ok = true });
    }

    /// <summary>Xoá 1 animation (dùng cho agent dọn item sinh lỗi/hỏng).</summary>
    [HttpDelete("animations/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _animations.GetByIdAsync(id);
        if (item is null)
            return NotFound(new { error = "Animation not found." });
        await _animations.DeleteAsync(id);
        return Ok(new { ok = true, deleted = item.Slug });
    }
}
