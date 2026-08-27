using MongoDB.Bson;
using MongoDB.Driver;
using UI_UX_pro.Data;
using UI_UX_pro.Models;

namespace UI_UX_pro.Services;

public class AnimationService
{
    private readonly IMongoCollection<AnimationItem> _items;

    public AnimationService(MongoDbContext db)
    {
        _items = db.Animations;
    }

    public async Task<AnimationItem?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var oid)) return null;
        return await _items.Find(x => x.Id == oid).FirstOrDefaultAsync();
    }

    public async Task<AnimationItem?> GetBySlugAsync(string slug)
    {
        return await _items.Find(x => x.Slug == slug).FirstOrDefaultAsync();
    }

    public async Task<(List<AnimationItem> Items, long Total)> SearchAsync(
        string? category = null,
        string? search = null,
        string? sort = "newest",
        int page = 1,
        int pageSize = 12,
        bool includeNonPublic = false)
    {
        var filterBuilder = Builders<AnimationItem>.Filter;
        var filters = new List<FilterDefinition<AnimationItem>>();

        if (!includeNonPublic)
            filters.Add(filterBuilder.Eq(x => x.Status, ItemStatus.Published) & filterBuilder.Eq(x => x.IsPublic, true));

        if (!string.IsNullOrWhiteSpace(category) && category != "all")
        {
            var catFilter = filterBuilder.Eq(x => x.CategorySlug, category.ToLowerInvariant());
            filters.Add(catFilter);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            var regex = new BsonRegularExpression(System.Text.RegularExpressions.Regex.Escape(s), "i");
            filters.Add(filterBuilder.Or(
                filterBuilder.Regex(x => x.Name, regex),
                filterBuilder.Regex(x => x.Description, regex),
                filterBuilder.Regex(x => x.Tags, regex),
                filterBuilder.Regex(x => x.CategoryName, regex)
            ));
        }

        var filter = filters.Count == 0 ? filterBuilder.Empty : filterBuilder.And(filters);

        var total = await _items.CountDocumentsAsync(filter);

        SortDefinition<AnimationItem> sortDef = sort switch
        {
            "popular" => Builders<AnimationItem>.Sort.Descending(x => x.ViewCount),
            "liked" => Builders<AnimationItem>.Sort.Descending(x => x.LikeCount),
            "name" => Builders<AnimationItem>.Sort.Ascending(x => x.Name),
            _ => Builders<AnimationItem>.Sort.Descending(x => x.CreatedAt)
        };

        var items = await _items.Find(filter)
            .Sort(sortDef)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<List<AnimationItem>> GetAllAsync(bool includeNonPublic = true)
    {
        var filter = includeNonPublic
            ? Builders<AnimationItem>.Filter.Empty
            : Builders<AnimationItem>.Filter.Eq(x => x.Status, ItemStatus.Published);
        return await _items.Find(filter).SortByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<long> CountAllAsync()
    {
        return await _items.CountDocumentsAsync(Builders<AnimationItem>.Filter.Empty);
    }

    public async Task CreateAsync(AnimationItem item)
    {
        item.Id = ObjectId.GenerateNewId();
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        item.ContentHash ??= ComputeContentHash(item.Html, item.Css, item.Js);
        await _items.InsertOneAsync(item);
    }

    public async Task UpdateAsync(AnimationItem item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        await _items.ReplaceOneAsync(x => x.Id == item.Id, item);
    }

    public async Task DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var oid)) return;
        await _items.DeleteOneAsync(x => x.Id == oid);
    }

    public async Task IncrementViewAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var oid)) return;
        var update = Builders<AnimationItem>.Update.Inc(x => x.ViewCount, 1);
        await _items.UpdateOneAsync(x => x.Id == oid, update);
    }

    public async Task IncrementLikeAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var oid)) return;
        var update = Builders<AnimationItem>.Update.Inc(x => x.LikeCount, 1);
        await _items.UpdateOneAsync(x => x.Id == oid, update);
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        return await _items.Find(x => x.Slug == slug).AnyAsync();
    }

    public static string Slugify(string name)
    {
        var normalized = name.ToLowerInvariant().Trim();
        var sb = new System.Text.StringBuilder();
        foreach (var c in normalized)
        {
            if (char.IsLetterOrDigit(c)) sb.Append(c);
            else if (c is ' ' or '-' or '_') sb.Append('-');
        }
        var slug = sb.ToString().Trim('-');
        while (slug.Contains("--")) slug = slug.Replace("--", "-");
        return string.IsNullOrEmpty(slug) ? Guid.NewGuid().ToString("N")[..8] : slug;
    }

    public async Task<string> EnsureUniqueSlugAsync(string baseSlug)
    {
        var slug = baseSlug;
        var i = 2;
        while (await SlugExistsAsync(slug))
        {
            slug = $"{baseSlug}-{i}";
            i++;
        }
        return slug;
    }

    /// <summary>Chuẩn hoá tên để so sánh trùng lặp: lowercase, trim, gộp khoảng trắng.</summary>
    public static string NormalizeName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "";
        return string.Join(' ', name.ToLowerInvariant().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    /// <summary>
    /// Hash nội dung (Html+Css+Js, bỏ toàn bộ khoảng trắng) để phát hiện item trùng nhau
    /// dù tên khác nhau — 2 component giống hệt nhau về mặt thị giác sẽ có cùng hash.
    /// </summary>
    public static string ComputeContentHash(string html, string css, string js)
    {
        static string Strip(string? s) => new((s ?? "").Where(c => !char.IsWhiteSpace(c)).ToArray());
        var raw = Strip(html) + "\u0001" + Strip(css) + "\u0001" + Strip(js);
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    /// <summary>Lấy N tên animation mới nhất (dùng để LLM tránh tạo trùng).</summary>
    public async Task<List<string>> GetRecentNamesAsync(int limit = 60)
    {
        return await _items.Find(_ => true)
            .SortByDescending(x => x.CreatedAt)
            .Limit(limit)
            .Project(x => x.Name)
            .ToListAsync();
    }

    /// <summary>Tính ContentHash cho các item cũ chưa có (chạy 1 lần khi app khởi động).</summary>
    public async Task BackfillContentHashesAsync()
    {
        var filter = Builders<AnimationItem>.Filter.Or(
            Builders<AnimationItem>.Filter.Eq(x => x.ContentHash, null),
            Builders<AnimationItem>.Filter.Eq(x => x.ContentHash, ""));
        var items = await _items.Find(filter).ToListAsync();
        foreach (var item in items)
        {
            item.ContentHash = ComputeContentHash(item.Html, item.Css, item.Js);
            await _items.ReplaceOneAsync(x => x.Id == item.Id, item);
        }
    }
}
