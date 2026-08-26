using MongoDB.Driver;
using UI_UX_pro.Data;
using UI_UX_pro.Models;

namespace UI_UX_pro.Services;

public class CategoryService
{
    private readonly IMongoCollection<Category> _categories;

    public CategoryService(MongoDbContext db)
    {
        _categories = db.Categories;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _categories.Find(x => x.IsActive).SortBy(x => x.Order).ToListAsync();
    }

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        return await _categories.Find(x => x.Slug == slug).FirstOrDefaultAsync();
    }

    public async Task UpsertAsync(Category category)
    {
        var existing = await _categories.Find(x => x.Slug == category.Slug).FirstOrDefaultAsync();
        if (existing is not null)
        {
            category.Id = existing.Id;
            await _categories.ReplaceOneAsync(x => x.Id == existing.Id, category);
        }
        else
        {
            category.Id = MongoDB.Bson.ObjectId.GenerateNewId();
            category.CreatedAt = DateTime.UtcNow;
            await _categories.InsertOneAsync(category);
        }
    }

    public async Task SeedDefaultsAsync(List<Category> defaults)
    {
        foreach (var c in defaults)
        {
            await UpsertAsync(c);
        }
    }
}
