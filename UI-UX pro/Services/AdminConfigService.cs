using System.Text.Json;
using System.Text.Json.Serialization;

namespace UI_UX_pro.Services;

/// <summary>
/// Cấu hình AI & Automation có thể chỉnh từ trang Admin và được lưu vào
/// file JSON ngoài appsettings.json (tránh phải build lại app mỗi lần đổi).
/// </summary>
public class AdminConfig
{
    public bool AutoEnabled { get; set; } = true;
    public bool AiEnabled { get; set; } = true;
    public string AiProvider { get; set; } = "openai";
    public string AiApiKey { get; set; } = "";
    public string AiEndpoint { get; set; } = "https://api.openai.com/v1/chat/completions";
    public string AiModel { get; set; } = "gpt-4o-mini";
    public int MaxItemsPerRun { get; set; } = 5;
    public List<string> TrendKeywords { get; set; } = new()
    {
        "glassmorphism", "micro-interaction", "3d tilt", "scroll reveal", "bento grid"
    };
    public string? Summary { get; set; }
}

public class AdminConfigService
{
    private readonly string _file;
    private readonly ILogger<AdminConfigService> _logger;
    private static readonly object _lock = new();

    public AdminConfigService(IWebHostEnvironment env, ILogger<AdminConfigService> logger)
    {
        // Lưu file ngoài wwwroot để không bị publish đè; trong thư mục data của project.
        _file = Path.Combine(env.ContentRootPath, "admin-config.json");
        _logger = logger;
    }

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    // Các giá trị mặc định đồng bộ với appsettings.json
    public AdminConfig GetDefaults(IConfiguration config)
    {
        return new AdminConfig
        {
            AutoEnabled = config.GetValue<bool>("Ai:Enabled", true),
            AiEnabled = config.GetValue<bool>("Ai:Enabled", true),
            AiApiKey = config.GetValue<string>("Ai:ApiKey") ?? "",
            AiEndpoint = config.GetValue<string>("Ai:Endpoint") ?? "https://api.openai.com/v1/chat/completions",
            AiModel = config.GetValue<string>("Ai:Model") ?? "gpt-4o-mini",
            MaxItemsPerRun = config.GetValue<int>("Ai:MaxItemsPerRun", 5),
            TrendKeywords = config.GetSection("DailyTrend:DefaultKeywords").Get<List<string>>() ?? new List<string>()
        };
    }

    /// <summary>Đọc cấu hình hiện tại; nếu file chưa tồn tại thì tạo từ appsettings mặc định.</summary>
    public AdminConfig Load(IConfiguration config)
    {
        lock (_lock)
        {
            if (File.Exists(_file))
            {
                try
                {
                    var json = File.ReadAllText(_file);
                    return JsonSerializer.Deserialize<AdminConfig>(json, _json) ?? GetDefaults(config);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to read admin-config.json, using defaults.");
                    return GetDefaults(config);
                }
            }

            var defaults = GetDefaults(config);
            Save(defaults);
            return defaults;
        }
    }

    public void Save(AdminConfig config)
    {
        lock (_lock)
        {
            var json = JsonSerializer.Serialize(config, _json);
            File.WriteAllText(_file, json);
        }
    }
}
