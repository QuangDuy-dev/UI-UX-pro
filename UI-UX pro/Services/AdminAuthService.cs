using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace UI_UX_pro.Services;

/// <summary>
/// Xác thực Admin đơn giản dựa trên Cookie + MasterPassword (không dùng bảng Account trong DB).
/// Cookie được ký bằng HMAC dựa trên secret máy chủ để chống giả mạo.
/// </summary>
public class AdminAuthService
{
    private readonly IConfiguration _config;
    private readonly byte[] _signingKey;

    public AdminAuthService(IConfiguration config)
    {
        _config = config;
        // Key ký cookie: dùng AdminSettings:CookieName + một secret ổn định. 
        // Trong môi trường production nên đặt khác nhau; tại mức demo dùng chuỗi cố định 32 bytes.
        var secret = config.GetValue<string>("AdminSettings:CookieName") ?? "UIAnimate_AdminAuth";
        _signingKey = SHA256.HashData(Encoding.UTF8.GetBytes("UIAnimate::AdminSign::" + secret));
    }

    public string CookieName => _config.GetValue<string>("AdminSettings:CookieName") ?? "UIAnimate_AdminAuth";

    public int CookieHours => _config.GetValue<int>("AdminSettings:CookieHours", 12);

    public bool VerifyPassword(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        var expected = _config.GetValue<string>("AdminSettings:MasterPassword") ?? "";
        if (string.IsNullOrWhiteSpace(expected)) return false;
        // So sánh thời gian cố định tránh timing attack
        var a = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)));
        var b = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(expected)));
        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
    }

    /// <summary>Kiểm tra cookie xác thực có hợp lệ (đúng chữ ký) hay không.</summary>
    public bool HasValidAuthCookie(HttpRequest request)
    {
        if (!request.Cookies.TryGetValue(CookieName, out var token) || string.IsNullOrWhiteSpace(token))
            return false;

        var parts = token.Split('.', 2);
        if (parts.Length != 2) return false;

        var payload = parts[0];
        var signature = parts[1];

        var expected = Sign(payload);
        var expectedHex = Convert.ToHexString(expected);
        var providedHex = signature; // đã là hex

        try
        {
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedHex),
                Encoding.UTF8.GetBytes(providedHex));
        }
        catch
        {
            return false;
        }
    }

    public void IssueCookie(HttpResponse response)
    {
        var payload = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        var signature = Convert.ToHexString(Sign(payload));
        var token = $"{payload}.{signature}";

        response.Cookies.Append(CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = false, // local HTTP dev
            IsEssential = true,
            Expires = DateTimeOffset.UtcNow.AddHours(CookieHours)
        });
    }

    public void ClearCookie(HttpResponse response)
    {
        response.Cookies.Delete(CookieName);
    }

    private byte[] Sign(string payload)
    {
        using var hmac = new HMACSHA256(_signingKey);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
    }
}
