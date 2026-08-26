using Microsoft.AspNetCore.Mvc;
using UI_UX_pro.Models;
using UI_UX_pro.Services;

namespace UI_UX_pro.Controllers;

public class UploadController : Controller
{
    private readonly AnimationService _animations;
    private readonly CategoryService _categories;

    public UploadController(AnimationService animations, CategoryService categories)
    {
        _animations = animations;
        _categories = categories;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["Categories"] = await _categories.GetAllAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(UploadViewModel model)
    {
        ViewData["Categories"] = await _categories.GetAllAsync();

        if (!ModelState.IsValid)
            return View(model);

        var category = await _categories.GetBySlugAsync(model.CategorySlug ?? "misc");
        var slug = await _animations.EnsureUniqueSlugAsync(AnimationService.Slugify(model.Name));

        var item = new AnimationItem
        {
            Name = model.Name.Trim(),
            Slug = slug,
            Description = model.Description,
            CategorySlug = category?.Slug ?? "misc",
            CategoryName = category?.Name ?? "Misc",
            Tags = (model.Tags ?? "").Split(',', ';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            Html = model.Html ?? "",
            Css = model.Css ?? "",
            Js = model.Js ?? "",
            Source = AnimationSource.User,
            Status = ItemStatus.Published,
            IsPublic = true
        };

        await _animations.CreateAsync(item);
        TempData["Flash"] = "✅ Animation của bạn đã được thêm vào gallery!";
        return RedirectToAction("Detail", "Home", new { slug = item.Slug });
    }
}
