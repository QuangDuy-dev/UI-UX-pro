using System.Web;
using UI_UX_pro.Models;

namespace UI_UX_pro.Services;

public static class PreviewBuilder
{
    /// <summary>
    /// Ghép Html + Css + Js thành một document hoàn chỉnh để nhúng vào iframe srcdoc.
    /// Tự động bọc <style>/<script> nếu code thuần, giữ nguyên nếu đã có thẻ.
    /// </summary>
    public static string BuildFullDocument(AnimationItem item)
    {
        var css = item.Css;
        if (!css.Contains("<style", StringComparison.OrdinalIgnoreCase))
            css = $"<style>\n{css}\n</style>";

        var js = item.Js;
        if (!string.IsNullOrWhiteSpace(js) && !js.Contains("<script", StringComparison.OrdinalIgnoreCase))
            js = $"<script>\n{js}\n</script>";

        return $"""
            <!DOCTYPE html>
            <html>
            <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1">
            {css}
            </head>
            <body>
            {item.Html}
            {js}
            </body>
            </html>
            """;
    }

    /// <summary>Phiên bản đã escape HTML cho thuộc tính srcdoc.</summary>
    public static string BuildSrcdoc(AnimationItem item)
    {
        return HttpUtility.HtmlAttributeEncode(BuildFullDocument(item));
    }

    /// <summary>Chỉ lấy phần CSS/HTML/JS để hiển thị trong tab code.</summary>
    public static string Highlighted(string code) => HttpUtility.HtmlEncode(code);
}
