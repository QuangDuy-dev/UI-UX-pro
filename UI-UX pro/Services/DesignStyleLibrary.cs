namespace UI_UX_pro.Services;

/// <summary>
/// Kho phong cách thiết kế đa dạng dùng để xoay vòng cho job hằng ngày.
/// Mỗi style thuộc 1 "family" — một ngày sẽ chọn các style thuộc family KHÁC NHAU
/// để web luôn có đủ mọi loại phong cách, không lặp lại 1-2 keyword quen thuộc.
/// </summary>
public sealed record DesignStyle(string Keyword, string Family, string[] Aliases, string Description);

public static class DesignStyleLibrary
{
    public static readonly IReadOnlyList<DesignStyle> All = new List<DesignStyle>
    {
        new("glassmorphism", "Glass", new[] { "glass", "frosted", "blur", "translucent" }, "Kính mờ, backdrop-filter blur"),
        new("neumorphism", "Soft UI", new[] { "neu", "soft ui", "emboss", "soft shadow" }, "UI mềm, shadow lõm/nổi"),
        new("claymorphism", "Clay", new[] { "clay", "soft 3d", "playful 3d" }, "Đất sét 3D mềm mại"),
        new("skeuomorphism", "Realistic", new[] { "skeuomorph", "realistic", "physical" }, "Mô phỏng vật thật"),
        new("flat design", "Flat", new[] { "flat", "2d", "minimal flat" }, "Phẳng, không bóng đổ"),
        new("material design", "Material", new[] { "material", "elevation", "ripple" }, "Google Material, elevation"),
        new("brutalism", "Brutalist", new[] { "brutal", "raw", "hard shadow", "thick border" }, "Thô ráp, bóng đổ cứng"),
        new("minimalism", "Minimal", new[] { "minimal", "whitespace", "less is more" }, "Tối giản, nhiều khoảng trống"),
        new("swiss design", "Swiss", new[] { "swiss", "international typographic", "helvetica", "grid" }, "Phong cách Thuỵ Sĩ, typography lưới"),
        new("bauhaus", "Bauhaus", new[] { "bauhaus", "geometric", "primary color" }, "Hình học, màu cơ bản"),
        new("art deco", "Art Deco", new[] { "deco", "gold", "luxury", "symmetry" }, "Sang trọng, vàng, đối xứng"),
        new("cyberpunk", "Cyberpunk", new[] { "cyber", "neon", "dystopian", "glitch city" }, "Neon tương lai, glitch"),
        new("vaporwave", "Vaporwave", new[] { "vapor", "retro 90s", "pink purple", "statue" }, "Retro 90s, hồng tím"),
        new("synthwave", "Synthwave", new[] { "synth", "retro 80s", "outrun", "sun grid" }, "Retro 80s, mặt trời lưới"),
        new("y2k", "Y2K", new[] { "y2k", "2000s", "chrome", "futuristic glossy" }, "2000s bóng loáng"),
        new("retro-futurism", "Retro Futurism", new[] { "space age", "atomic", "retro future" }, "Tương lai hoài cổ"),
        new("pixel art", "Pixel", new[] { "pixel", "8-bit", "16-bit", "retro game" }, "Game retro 8-bit"),
        new("low poly", "Low Poly", new[] { "lowpoly", "faceted", "geometric 3d" }, "3D đa giác thấp"),
        new("isometric", "Isometric", new[] { "iso", "axonometric", "2.5d" }, "Góc nhìn 2.5D"),
        new("3d tilt", "3D", new[] { "tilt", "perspective", "3d card" }, "Card nghiêng 3D theo chuột"),
        new("parallax", "Parallax", new[] { "parallax scroll", "depth", "layered scroll" }, "Cuộn nhiều lớp chiều sâu"),
        new("scroll-triggered", "Scroll", new[] { "scroll reveal", "scroll animation", "on-scroll" }, "Hiệu ứng kích hoạt khi cuộn"),
        new("kinetic typography", "Kinetic Type", new[] { "kinetic", "moving type", "animated text" }, "Chữ chuyển động"),
        new("marquee", "Marquee", new[] { "ticker", "scrolling strip", "infinite text" }, "Dải chữ chạy vô tận"),
        new("bento grid", "Bento", new[] { "bento", "modular grid", "dashboard cards" }, "Lưới bento kiểu dashboard"),
        new("aurora", "Aurora", new[] { "aurora gradient", "soft gradient", "northern lights" }, "Gradient cực quang"),
        new("gradient mesh", "Gradient Mesh", new[] { "mesh gradient", "vivid blur", "colorful gradient" }, "Gradient lưới màu"),
        new("duotone", "Duotone", new[] { "duo", "two color", "spot color" }, "Hai tông màu"),
        new("grain texture", "Grain", new[] { "noise", "film grain", "textured" }, "Hạt nhiễu phim"),
        new("halftone", "Halftone", new[] { "dots", "print dots", "comic" }, "Chấm in báo"),
        new("holographic", "Holographic", new[] { "holo", "iridescent", "shifting rainbow" }, "Óng ánh đổi màu"),
        new("liquid", "Liquid", new[] { "liquid metal", "gooey", "melting" }, "Kim loại lỏng"),
        new("organic blob", "Organic", new[] { "blob", "amorphous", "wavy shapes" }, "Hình blob hữu cơ"),
        new("morphing", "Morph", new[] { "morph", "shape shift", "transition" }, "Biến hình chuyển động"),
        new("dark mode", "Dark", new[] { "dark", "night", "black", "midnight" }, "Giao diện tối"),
        new("pastel", "Pastel", new[] { "pastel", "soft candy", "light colors" }, "Màu pastel ngọt"),
        new("monochrome", "Mono", new[] { "black white", "grayscale", "mono" }, "Đen trắng"),
        new("high contrast", "High Contrast", new[] { "contrast", "bold", "black white yellow" }, "Tương phản cao"),
        new("outline", "Outline", new[] { "stroke", "line art", "hollow" }, "Viền nét"),
        new("sticker", "Sticker", new[] { "sticker", "die cut", "cute" }, "Nhãn dán dễ thương"),
        new("neon glow", "Neon", new[] { "neon sign", "glow", "luminous" }, "Đèn neon phát sáng"),
        new("glowing shadow", "Glow", new[] { "glow", "luminous edges", "light" }, "Bóng phát sáng"),
        new("layered cards", "Layered", new[] { "stacked", "deck", "offset layers" }, "Thẻ xếp lớp"),
        new("flip card", "Flip", new[] { "flip", "3d flip", "rotate card" }, "Thẻ lật 3D"),
        new("magnetic hover", "Magnetic", new[] { "magnet", "cursor attract" }, "Hút theo chuột"),
        new("cursor-follow", "Cursor", new[] { "cursor", "pointer trail", "mouse" }, "Đuổi theo con trỏ"),
        new("confetti", "Confetti", new[] { "celebration", "burst", "party" }, "Bắn pháo giấy"),
        new("particles", "Particles", new[] { "particle field", "dots", "bokeh" }, "Hạt bay"),
        new("starfield", "Starfield", new[] { "stars", "space", "twinkle" }, "Trời sao"),
        new("glitch", "Glitch", new[] { "rgb split", "error", "static" }, "Lỗi RGB tách kênh"),
        new("crt scanline", "CRT", new[] { "crt", "scanline", "retro monitor" }, "Màn hình CRT"),
        new("terminal", "Terminal", new[] { "console", "cli", "monospace", "hacker" }, "Phong cách terminal"),
        new("editorial", "Editorial", new[] { "magazine", "big serif type", "fashion" }, "Tạp chí thời trang"),
        new("dotted grid", "Dotted", new[] { "dots", "blueprint", "dot grid" }, "Lưới chấm"),
        new("stripes", "Stripes", new[] { "striped", "barber", "tape" }, "Sọc kẻ"),
        new("checkered", "Checkered", new[] { "checkerboard", "chess", "racing" }, "Bàn cờ"),
        new("zigzag", "Zigzag", new[] { "zig zag", "sawtooth", "lightning" }, "Răng cưa"),
        new("fluid", "Fluid", new[] { "flowing gradient", "waves", "liquid gradient" }, "Chất lỏng chảy"),
        new("wave", "Wave", new[] { "ocean wave", "sound wave", "sine" }, "Sóng biển / sóng âm"),
        new("metallic", "Metallic", new[] { "metal", "chrome", "silver" }, "Kim loại chrome"),
        new("gold foil", "Gold", new[] { "gold", "luxury", "premium" }, "Vàng lá"),
        new("shimmer", "Shimmer", new[] { "shine", "sweep", "sparkle" }, "Lấp lánh quét ngang"),
        new("skeleton", "Skeleton", new[] { "skeleton loading", "placeholder", "shimmer block" }, "Loading skeleton"),
        new("typewriter", "Typewriter", new[] { "typing", "typed", "cursor blink" }, "Chữ đánh máy"),
        new("text reveal", "Text Reveal", new[] { "mask reveal", "stagger text", "word reveal" }, "Chữ hiện dần theo khối"),
        new("floating", "Floating", new[] { "float", "levitate", "hover drift" }, "Trôi nổi nhẹ nhàng"),
        new("shake", "Shake", new[] { "shake", "vibrate", "wobble" }, "Rung lắc"),
        new("pulse", "Pulse", new[] { "pulse", "heartbeat", "breathing" }, "Nhịp đập"),
        new("bounce", "Bounce", new[] { "bounce", "jump", "ball" }, "Nảy"),
        new("swing", "Swing", new[] { "pendulum", "swing", "dangle" }, "Đu đưa"),
        new("rotate", "Rotate", new[] { "spin", "orbit", "circular" }, "Xoay vòng"),
        new("gradient border", "Gradient Border", new[] { "animated border", "border glow" }, "Viền gradient chuyển động"),
        new("spotlight", "Spotlight", new[] { "light beam", "radial", "lamp" }, "Chùm đèn chiếu"),
        new("noise", "Noise", new[] { "static", "grain", "texture" }, "Nhiễu tĩnh"),
        new("card stack", "Stack", new[] { "stacked cards", "deck", "hover stack" }, "Chồng thẻ xếp lớp"),
        new("tilt card", "Tilt", new[] { "mouse tilt", "perspective hover", "rotate card" }, "Thẻ nghiêng theo chuột"),
        new("morphing gradient", "Morph Gradient", new[] { "animated gradient", "color shift", "gradient move" }, "Gradient biến đổi màu"),
        new("orbit", "Orbit", new[] { "planets", "satellites", "circles" }, "Quỹ đạo hành tinh"),
    };

    /// <summary>
    /// Chọn `count` style chưa dùng gần đây, xoay vòng theo ngày (dayOffset) để mỗi ngày
    /// ra các style khác nhau, mỗi style thuộc family khác nhau cho tới khi hết.
    /// </summary>
    public static List<string> PickRotation(IReadOnlyCollection<string> recentlyUsed, int count, int dayOffset)
    {
        var used = new HashSet<string>(recentlyUsed.Select(k => k.Trim().ToLowerInvariant()), StringComparer.OrdinalIgnoreCase);

        var candidates = All.Where(s => !IsUsed(s, used)).ToList();
        if (candidates.Count < count)
            candidates = All.Where(s => !used.Contains(s.Keyword.ToLowerInvariant())).ToList(); // bỏ lọc alias nếu thiếu
        if (candidates.Count == 0)
            candidates = All.ToList();

        var start = Math.Abs(dayOffset) % candidates.Count;
        var picked = new List<DesignStyle>();
        var families = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Vòng 1: mỗi family 1 style (đảm bảo đa dạng phong cách)
        for (int i = 0; i < candidates.Count && picked.Count < count; i++)
        {
            var s = candidates[(start + i) % candidates.Count];
            if (families.Add(s.Family)) picked.Add(s);
        }
        // Vòng 2: lấp chỗ còn thiếu bằng các style kế tiếp
        for (int i = 0; i < candidates.Count && picked.Count < count; i++)
        {
            var s = candidates[(start + i) % candidates.Count];
            if (!picked.Contains(s)) picked.Add(s);
        }

        return picked.Select(s => s.Keyword).ToList();
    }

    private static bool IsUsed(DesignStyle s, HashSet<string> used)
    {
        if (used.Contains(s.Keyword.ToLowerInvariant())) return true;
        foreach (var a in s.Aliases)
            if (used.Contains(a.ToLowerInvariant())) return true;
        return false;
    }
}
