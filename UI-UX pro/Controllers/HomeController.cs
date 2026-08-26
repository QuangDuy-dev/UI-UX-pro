using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UI_UX_pro.Models;
using UI_UX_pro.Services;

namespace UI_UX_pro.Controllers;

public class HomeController : Controller
{
    private readonly AnimationService _animations;
    private readonly CategoryService _categories;

    public HomeController(AnimationService animations, CategoryService categories)
    {
        _animations = animations;
        _categories = categories;
    }

    public async Task<IActionResult> Index(
        string? category = null,
        string? q = null,
        string? sort = "newest",
        int page = 1)
    {
        var cats = await _categories.GetAllAsync();
        var (items, total) = await _animations.SearchAsync(category, q, sort, page, pageSize: 12);

        ViewData["Categories"] = cats;
        ViewData["CurrentCategory"] = category ?? "all";
        ViewData["CurrentQuery"] = q ?? "";
        ViewData["CurrentSort"] = sort ?? "newest";
        ViewData["TotalItems"] = total;
        ViewBag.Page = page;

        return View(items);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        var item = await _animations.GetBySlugAsync(slug);
        if (item is null || item.Status != ItemStatus.Published || !item.IsPublic)
            return NotFound();

        await _animations.IncrementViewAsync(item.Id.ToString());

        var cats = await _categories.GetAllAsync();
        ViewData["Categories"] = cats;

        // Related items cùng category
        var (related, _) = await _animations.SearchAsync(item.CategorySlug, null, "popular", 1, 6);
        related = related.Where(r => r.Id != item.Id).ToList();
        ViewData["Related"] = related;

        return View(item);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
