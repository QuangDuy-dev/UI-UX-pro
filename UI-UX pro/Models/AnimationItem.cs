using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UI_UX_pro.Models;

public class AnimationItem
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Slug { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public string CategorySlug { get; set; } = "";

    public string CategoryName { get; set; } = "";

    public List<string> Tags { get; set; } = new();

    public string Html { get; set; } = "";

    public string Css { get; set; } = "";

    public string Js { get; set; } = "";

    /// <summary>SHA-256 của Html+Css+Js (đã bỏ khoảng trắng) — dùng để chống trùng lặp nội dung.</summary>
    public string? ContentHash { get; set; }

    public AnimationSource Source { get; set; } = AnimationSource.User;

    public ItemStatus Status { get; set; } = ItemStatus.Published;

    public bool IsPublic { get; set; } = true;

    public long ViewCount { get; set; }

    public long LikeCount { get; set; }

    public List<string> TrendKeywords { get; set; } = new();

    public string? TrendSourceUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
