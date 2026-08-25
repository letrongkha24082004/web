using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManager.Data;
using ShopManager.Models;
using ShopManager.Models.ViewModels;

namespace ShopManager.Controllers;

public class HomeController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? search, int? categoryId, string sort = "newest", int page = 1)
    {
        const int pageSize = 8;
        page = Math.Max(1, page);
        var query = db.Products.AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.IsActive && x.Category!.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.Name.Contains(term) || x.Description.Contains(term));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        query = sort switch
        {
            "price-asc" => query.OrderBy(x => x.Price),
            "price-desc" => query.OrderByDescending(x => x.Price),
            "name" => query.OrderBy(x => x.Name),
            _ => query.OrderByDescending(x => x.IsFeatured).ThenByDescending(x => x.CreatedAt)
        };

        var count = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(count / (double)pageSize));
        page = Math.Min(page, totalPages);
        var model = new StoreIndexViewModel
        {
            Products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(),
            Categories = await db.Categories.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(),
            Search = search,
            CategoryId = categoryId,
            Sort = sort,
            Page = page,
            TotalPages = totalPages
        };
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult HttpStatus(int code)
    {
        Response.StatusCode = code;
        return View("StatusCode", code);
    }
}
