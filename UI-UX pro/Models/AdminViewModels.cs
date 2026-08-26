using System.ComponentModel.DataAnnotations;

namespace UI_UX_pro.Models;

public class AdminLoginViewModel
{
    [Required]
    public string Password { get; set; } = "";
}

public class SettingsViewModel
{
    public bool AutoEnabled { get; set; } = true;
    public bool AiEnabled { get; set; } = true;
    public string AiProvider { get; set; } = "openai";
    public string AiApiKey { get; set; } = "";
    public string AiEndpoint { get; set; } = "https://api.openai.com/v1/chat/completions";
    public string AiModel { get; set; } = "gpt-4o-mini";
    public int MaxItemsPerRun { get; set; } = 5;
    public string TrendKeywords { get; set; } = "";
    public string? Summary { get; set; }
}
