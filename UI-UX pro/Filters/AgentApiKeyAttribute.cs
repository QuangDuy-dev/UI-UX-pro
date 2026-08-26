using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UI_UX_pro.Filters;

/// <summary>
/// Bảo vệ API dành cho Agent (cron job):
/// - Nếu appsettings ApiSecurity:AgentKey KHÔNG rỗng → yêu cầu header "X-Api-Key" khớp, sai/thiếu trả 401.
/// - Nếu rỗng (mặc định) → cho phép truy cập công khai, Agent gọi tự do không bị chặn.
/// Không ảnh hưởng tới các trang Admin (AdminController dùng [AdminAuthorize] riêng).
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AgentApiKeyAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var config = context.HttpContext.RequestServices.GetService<IConfiguration>();
        var expected = config?.GetValue<string>("ApiSecurity:AgentKey");

        // Chế độ công khai (mặc định): không cấu hình key → cho qua.
        if (string.IsNullOrWhiteSpace(expected))
            return;

        var provided = context.HttpContext.Request.Headers["X-Api-Key"].ToString();
        if (string.IsNullOrWhiteSpace(provided) || !FixedTimeEquals(expected.Trim(), provided.Trim()))
            context.Result = new StatusCodeResult(401);
    }

    private static bool FixedTimeEquals(string a, string b)
    {
        var ba = System.Text.Encoding.UTF8.GetBytes(a);
        var bb = System.Text.Encoding.UTF8.GetBytes(b);
        return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(ba, bb);
    }
}
