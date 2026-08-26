using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using UI_UX_pro.Data;
using UI_UX_pro.Filters;
using UI_UX_pro.Models;
using UI_UX_pro.Services;

namespace UI_UX_pro.Controllers;

/// <summary>
/// AdminController nằm sau route bí mật (đọc từ appsettings: AdminSettings:SecretRoute).
/// Mọi action đều cần cookie xác thực hợp lệ (xem [AdminAuthorize]).
/// </summary>
[Route("Admin-Secret-Manager-Key-999")]
[AdminAuthorize]
public class AdminController : Controller
{
    private readonly AnimationService _animations;
    private readonly CategoryService _categories;
    private readonly TrendService _trends;
    private readonly MongoDbContext _db;
    private readonly AdminConfigService _configService;
    private readonly AdminAuthService _auth;
    private readonly IConfiguration _config;

    public AdminController(
        AnimationService animations,
        CategoryService categories,
        TrendService trends,
        MongoDbContext db,
        AdminConfigService configService,
        AdminAuthService auth,
        IConfiguration config)
    {
        _animations = animations;
        _categories = categories;
        _trends = trends;
        _db = db;
        _configService = configService;
        _auth = auth;
        _config = config;
    }

    // ---- Trang đăng nhập nhỏ (không nằm trong [AdminAuthorize] vì filter áp cho class) ----
    // Để cho phép truy cập Login mà không cần cookie, ta ghi đè attribute ở action.
    [AllowAnonymous]
    [Route("login")]
    [HttpGet]
    public IActionResult Login()
    {
        if (_auth.HasValidAuthCookie(Request))
        {
            var route = secretRoute();
            return Redirect(route);
        }
        return View(new AdminLoginViewModel());
    }

    [AllowAnonymous]
    [Route("login")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(AdminLoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (_auth.VerifyPassword(model.Password))
        {
            _auth.IssueCookie(Response);
            var route = secretRoute();
            return Redirect(route);
        }

        ViewData["Error"] = "Mật khẩu không đúng.";
        return View(model);
    }

    [Route("logout")]
    [HttpPost]
    public IActionResult Logout()
    {
        _auth.ClearCookie(Response);
        return RedirectToAction("Index", "Home");
    }

    // ---- Khu vực quản lý (đã được bảo vệ bởi [AdminAuthorize]) ----

    [Route("")]
    public async Task<IActionResult> Index()
    {
        var items = await _animations.GetAllAsync(includeNonPublic: true);
        var config = _configService.Load(_config);
        ViewData["TotalItems"] = items.Count;
        ViewData["TotalViews"] = items.Sum(i => i.ViewCount);
        ViewData["TotalLikes"] = items.Sum(i => i.LikeCount);
        ViewData["DailyCount"] = items.Count(i => i.Source == AnimationSource.Daily);
        ViewData["AiCount"] = items.Count(i => i.Source == AnimationSource.Ai);
        ViewData["AutoEnabled"] = config.AutoEnabled;
        ViewData["MasterPassword"] = _config.GetValue<string>("AdminSettings:MasterPassword");
        ViewData["SecretRoute"] = secretRoute();
        return View(items);
    }

    [Route("trends")]
    public async Task<IActionResult> Trends()
    {
        var reports = await _db.TrendReports.Find(_ => true).SortByDescending(r => r.RunDate).Limit(30).ToListAsync();
        return View(reports);
    }

    [Route("settings")]
    public IActionResult Settings()
    {
        var config = _configService.Load(_config);
        var vm = new SettingsViewModel
        {
            AutoEnabled = config.AutoEnabled,
            AiEnabled = config.AiEnabled,
            AiProvider = config.AiProvider,
            AiApiKey = config.AiApiKey,
            AiEndpoint = config.AiEndpoint,
            AiModel = config.AiModel,
            MaxItemsPerRun = config.MaxItemsPerRun,
            TrendKeywords = string.Join(", ", config.TrendKeywords),
            Summary = config.Summary
        };
        ViewData["SecretRoute"] = secretRoute();
        ViewData["MasterPassword"] = _config.GetValue<string>("AdminSettings:MasterPassword");
        return View(vm);
    }

    [Route("settings")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Settings(SettingsViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewData["SecretRoute"] = secretRoute();
            ViewData["MasterPassword"] = _config.GetValue<string>("AdminSettings:MasterPassword");
            return View(vm);
        }

        var config = _configService.Load(_config);
        config.AutoEnabled = vm.AutoEnabled;
        config.AiEnabled = vm.AiEnabled;
        config.AiProvider = vm.AiProvider ?? "openai";
        config.AiApiKey = vm.AiApiKey ?? "";
        config.AiEndpoint = string.IsNullOrWhiteSpace(vm.AiEndpoint)
            ? DefaultEndpointFor(config.AiProvider)
            : vm.AiEndpoint.Trim();
        config.AiModel = vm.AiModel ?? "gpt-4o-mini";
        config.MaxItemsPerRun = Math.Clamp(vm.MaxItemsPerRun, 1, 20);
        config.TrendKeywords = (vm.TrendKeywords ?? "")
            .Split(',', ';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
        config.Summary = vm.Summary;
        _configService.Save(config);

        TempData["Flash"] = "✅ Đã lưu cài đặt Settings & Automation.";
        return RedirectToAction(nameof(Settings));
    }

    [Route("run-trend-job")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RunTrendJob(string? keywords = null)
    {
        var config = _configService.Load(_config);

        // Áp dụng nhanh cấu hình từ file vào IConfiguration (Ai:Enabled, MaxItems, ApiKey, Endpoint, Model)
        ApplyAiConfig(config);

        var kw = string.IsNullOrWhiteSpace(keywords)
            ? (config.TrendKeywords.Count > 0 ? config.TrendKeywords : null)
            : keywords.Split(',', ';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        var report = await _trends.GenerateDailyAsync(kw, config.Summary, maxItems: config.MaxItemsPerRun);

        if (report.Status == "success")
            TempData["Flash"] = $"✅ Chạy thành công: tạo {report.GeneratedItemIds.Count} animation mới.";
        else if (report.Status == "empty")
            TempData["Flash"] = "⚠️ Không tạo được animation nào (AI sinh rỗng). Kiểm tra API key / model.";
        else
            TempData["Flash"] = $"❌ Lỗi: {report.Error}";

        return RedirectToAction(nameof(Trends));
    }

    [Route("toggle-publish")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePublish(string id)
    {
        var item = await _animations.GetByIdAsync(id);
        if (item is not null)
        {
            item.Status = item.Status == ItemStatus.Published ? ItemStatus.Archived : ItemStatus.Published;
            await _animations.UpdateAsync(item);
        }
        return RedirectToAction(nameof(Index));
    }

    [Route("delete")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        await _animations.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [Route("run-daily")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RunDaily(string? keywords)
    {
        var config = _configService.Load(_config);
        ApplyAiConfig(config);
        var kw = string.IsNullOrWhiteSpace(keywords) ? null
            : keywords.Split(',', ';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        var report = await _trends.GenerateDailyAsync(kw, config.Summary, maxItems: config.MaxItemsPerRun);
        TempData["Flash"] = $"✅ Đã tạo {report.GeneratedItemIds.Count} animation mới từ trends ({report.Status}).";
        return RedirectToAction(nameof(Trends));
    }

    // ---- Helpers ----

    private string secretRoute()
    {
        return "/" + (_config.GetValue<string>("AdminSettings:SecretRoute") ?? "Admin-Secret-Manager-Key-999");
    }

    private static string DefaultEndpointFor(string provider) => provider switch
    {
        "openai" => "https://api.openai.com/v1/chat/completions",
        "deepseek" => "https://api.deepseek.com/v1/chat/completions",
        "anthropic" => "https://api.anthropic.com/v1/messages",
        _ => "https://api.openai.com/v1/chat/completions"
    };

    private void ApplyAiConfig(AdminConfig config)
    {
        // Ghi đè IConfiguration bằng giá trị từ file admin-config.json để TrendService dùng đúng.
        _config["Ai:Enabled"] = config.AiEnabled ? "true" : "false";
        _config["Ai:Model"] = config.AiModel;
        _config["Ai:MaxItemsPerRun"] = config.MaxItemsPerRun.ToString();
    }
}
