using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UI_UX_pro.Models;

public class TrendReport
{
    [BsonId]
    public ObjectId Id { get; set; }

    public DateTime RunDate { get; set; } = DateTime.UtcNow;

    public List<string> Keywords { get; set; } = new();

    public string? Summary { get; set; }

    public List<string> SourceUrls { get; set; } = new();

    public List<string> GeneratedItemIds { get; set; } = new();

    /// <summary>Số item bị bỏ qua vì trùng lặp (tên hoặc nội dung đã tồn tại).</summary>
    public int SkippedCount { get; set; }

    /// <summary>Lý do bỏ qua từng item trùng.</summary>
    public List<string> SkippedReasons { get; set; } = new();

    public string Status { get; set; } = "success";

    public string? Error { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
