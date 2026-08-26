using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UI_UX_pro.Services;

namespace UI_UX_pro.Filters;

/// <summary>
/// Attribute áp lên AdminController: chặn truy cập nếu không có cookie xác thực hợp lệ.
/// Action Login/Logout được đánh dấu [AllowAnonymous] sẽ được bỏ qua.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Cho phép action được đánh dấu [AllowAnonymous] (vd: Login) đi qua.
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .Any(em => em.GetType().Name == "AllowAnonymousAttribute");
        if (allowAnonymous)
            return;

        var auth = context.HttpContext.RequestServices.GetService<AdminAuthService>();
        if (auth is null || !auth.HasValidAuthCookie(context.HttpContext.Request))
        {
            // Nếu là AJAX / API trong admin thì trả 403; còn lại redirect về login của route bí mật.
            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.Result = new StatusCodeResult(403);
            }
            else
            {
                context.Result = new RedirectToActionResult("Login", "Admin", null);
            }
        }
    }
}
