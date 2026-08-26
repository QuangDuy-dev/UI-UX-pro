using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UI_UX_pro.Models;

public class Category
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Slug { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public string Icon { get; set; } = "📦";

    public int Order { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
