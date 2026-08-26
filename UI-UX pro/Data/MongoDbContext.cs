using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UI_UX_pro.Models;

namespace UI_UX_pro.Data;

public class MongoDbOptions
{
    public string DatabaseName { get; set; } = "uianimationdb";
}

public class MongoDbContext
{
    private readonly IMongoDatabase _db;

    public MongoDbContext(IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("MongoDb")
                 ?? throw new InvalidOperationException("Missing connection string 'MongoDb'.");

        var dbName = configuration.GetSection("MongoDb:DatabaseName").Value ?? "uianimationdb";

        var settings = MongoClientSettings.FromConnectionString(cs);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        var client = new MongoClient(settings);
        _db = client.GetDatabase(dbName);
    }

    public IMongoCollection<AnimationItem> Animations => _db.GetCollection<AnimationItem>("animations");
    public IMongoCollection<Category> Categories => _db.GetCollection<Category>("categories");
    public IMongoCollection<TrendReport> TrendReports => _db.GetCollection<TrendReport>("trendReports");
}
