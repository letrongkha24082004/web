using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManager.Data;
using ShopManager.Extensions;
using ShopManager.Models.Entities;
using ShopManager.Security;

namespace ShopManager.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class CategoriesController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.Categories.AsNoTracking()
        .Include(x => x.Products).OrderBy(x => x.Name).ToListAsync());

    public IActionResult Create() => View(new Category());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        ModelState.Remove(nameof(Category.Slug));
        if (await db.Categories.AnyAsync(x => x.Name == category.Name.Trim()))
            ModelState.AddModelError(nameof(category.Name), "Tên danh mục đã tồn tại.");
        if (!ModelState.IsValid) return View(category);
        category.Name = category.Name.Trim();
        category.Slug = await UniqueSlugAsync(category.Name);
        db.Categories.Add(category);
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã thêm danh mục.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await db.Categories.FindAsync(id);
        return category is null ? NotFound() : View(category);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category input)
    {
        if (id != input.Id) return BadRequest();
        ModelState.Remove(nameof(Category.Slug));
        if (await db.Categories.AnyAsync(x => x.Name == input.Name.Trim() && x.Id != id))
            ModelState.AddModelError(nameof(input.Name), "Tên danh mục đã tồn tại.");
        if (!ModelState.IsValid) return View(input);
        var category = await db.Categories.FindAsync(id);
        if (category is null) return NotFound();
        category.Name = input.Name.Trim();
        category.Slug = await UniqueSlugAsync(input.Name, id);
        category.Description = input.Description;
        category.IsActive = input.IsActive;
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật danh mục.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await db.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);
        if (category is null) return NotFound();
        if (category.Products.Count > 0)
        {
            TempData["Error"] = "Không thể xóa danh mục đang có sản phẩm.";
        }
        else
        {
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            TempData["Success"] = "Đã xóa danh mục.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> UniqueSlugAsync(string name, int? excludedId = null)
    {
        var baseSlug = name.ToSlug();
        var slug = baseSlug;
        var index = 2;
        while (await db.Categories.AnyAsync(x => x.Slug == slug && x.Id != excludedId)) slug = $"{baseSlug}-{index++}";
        return slug;
    }
}
