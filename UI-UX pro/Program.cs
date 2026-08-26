using System.Text.Json;
using UI_UX_pro.Data;
using UI_UX_pro.Models;
using UI_UX_pro.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// MongoDB
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<AnimationService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<TrendService>();

// Admin auth + config (cookie-based, không dùng DB Account)
builder.Services.AddScoped<AdminAuthService>();
builder.Services.AddSingleton<AdminConfigService>();

var app = builder.Build();

// Nạp cấu hình AI/automation từ admin-config.json (nếu có) vào IConfiguration
var adminCfg = app.Services.GetRequiredService<AdminConfigService>();
var cfg = adminCfg.Load(app.Configuration);
app.Configuration["Ai:Enabled"] = cfg.AiEnabled ? "true" : "false";
app.Configuration["Ai:ApiKey"] = cfg.AiApiKey;
app.Configuration["Ai:Endpoint"] = cfg.AiEndpoint;
app.Configuration["Ai:Model"] = cfg.AiModel;
app.Configuration["Ai:MaxItemsPerRun"] = cfg.MaxItemsPerRun.ToString();

// Seed: danh mục + template mẫu khi DB trống
await SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

static async Task SeedAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var categories = scope.ServiceProvider.GetRequiredService<CategoryService>();
    var animations = scope.ServiceProvider.GetRequiredService<AnimationService>();

    var defaults = new List<Category>
    {
        new() { Slug = "nav", Name = "Navigation", Icon = "🧭", Order = 1, Description = "Navbars, menus, breadcrumbs" },
        new() { Slug = "hero", Name = "Hero Section", Icon = "🦸", Order = 2, Description = "Landing hero & banners" },
        new() { Slug = "button", Name = "Button", Icon = "🔘", Order = 3, Description = "Animated buttons & CTAs" },
        new() { Slug = "card", Name = "Card", Icon = "🃏", Order = 4, Description = "Cards, tiles & panels" },
        new() { Slug = "list", Name = "List", Icon = "📋", Order = 5, Description = "Lists & timelines" },
        new() { Slug = "table", Name = "Table", Icon = "📊", Order = 6, Description = "Data tables & grids" },
        new() { Slug = "form", Name = "Form", Icon = "📝", Order = 7, Description = "Inputs, forms & validation" },
        new() { Slug = "loader", Name = "Loader", Icon = "⏳", Order = 8, Description = "Spinners & skeletons" },
        new() { Slug = "modal", Name = "Modal", Icon = "🪟", Order = 9, Description = "Dialogs & popups" },
        new() { Slug = "toast", Name = "Toast", Icon = "🔔", Order = 10, Description = "Notifications & toasts" },
        new() { Slug = "tabs", Name = "Tabs", Icon = "📑", Order = 11, Description = "Tabbed content" },
        new() { Slug = "accordion", Name = "Accordion", Icon = "🎹", Order = 12, Description = "Expandable sections" },
        new() { Slug = "badge", Name = "Badge", Icon = "🏷️", Order = 13, Description = "Badges & chips" },
        new() { Slug = "progress", Name = "Progress", Icon = "📈", Order = 14, Description = "Progress bars & steps" },
        new() { Slug = "carousel", Name = "Carousel", Icon = "🎠", Order = 15, Description = "Sliders & carousels" },
        new() { Slug = "marquee", Name = "Marquee", Icon = "✨", Order = 16, Description = "Infinite scrolling strips" },
        new() { Slug = "counter", Name = "Counter", Icon = "🔢", Order = 17, Description = "Animated numbers" },
        new() { Slug = "chat", Name = "Chat", Icon = "💬", Order = 18, Description = "Chat & messaging UI" },
        new() { Slug = "footer", Name = "Footer", Icon = "🦶", Order = 19, Description = "Footers & signoffs" },
        new() { Slug = "dropdown", Name = "Dropdown", Icon = "🔽", Order = 20, Description = "Dropdown menus" },
        new() { Slug = "pricing", Name = "Pricing", Icon = "💰", Order = 21, Description = "Pricing & plans" },
        new() { Slug = "scroll", Name = "Scroll Effect", Icon = "🎢", Order = 22, Description = "Scroll-triggered effects" },
        new() { Slug = "misc", Name = "Misc", Icon = "📦", Order = 99, Description = "Everything else" }
    };

    await categories.SeedDefaultsAsync(defaults);

    // Seed template vào gallery nếu chưa có item nào
    var count = await animations.CountAllAsync();
    if (count == 0)
    {
        foreach (var tpl in TemplateLibrary.All)
        {
            await animations.CreateAsync(new AnimationItem
            {
                Name = tpl.Name,
                Slug = await animations.EnsureUniqueSlugAsync(AnimationService.Slugify(tpl.Name)),
                Description = tpl.Description,
                CategorySlug = tpl.CategorySlug,
                CategoryName = tpl.CategoryName,
                Tags = tpl.Tags.ToList(),
                Html = tpl.Html,
                Css = tpl.Css,
                Js = tpl.Js,
                Source = AnimationSource.Template,
                Status = ItemStatus.Published,
                IsPublic = true
            });
        }
    }
}
