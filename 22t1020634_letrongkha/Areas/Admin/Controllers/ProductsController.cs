using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopManager.Data;
using ShopManager.Extensions;
using ShopManager.Models.Entities;
using ShopManager.Security;

namespace ShopManager.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class ProductsController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? search, int? categoryId)
    {
        var query = db.Products.AsNoTracking().Include(x => x.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search.Trim()));
        if (categoryId.HasValue) query = query.Where(x => x.CategoryId == categoryId);
        ViewBag.Search = search;
        ViewBag.CategoryId = new SelectList(await db.Categories.OrderBy(x => x.Name).ToListAsync(), "Id", "Name", categoryId);
        return View(await query.OrderByDescending(x => x.CreatedAt).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAsync();
        return View(new Product());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        ModelState.Remove(nameof(Product.Slug));
        if (!await db.Categories.AnyAsync(x => x.Id == product.CategoryId))
            ModelState.AddModelError(nameof(product.CategoryId), "Danh mục không tồn tại.");
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync(product.CategoryId);
            return View(product);
        }

        product.Name = product.Name.Trim();
        product.Slug = await UniqueSlugAsync(product.Name);
        product.CreatedAt = DateTime.UtcNow;
        db.Products.Add(product);
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã thêm sản phẩm mới.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return NotFound();
        await LoadCategoriesAsync(product.CategoryId);
        return View(product);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product input)
    {
        if (id != input.Id) return BadRequest();
        ModelState.Remove(nameof(Product.Slug));
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync(input.CategoryId);
            return View(input);
        }
        var product = await db.Products.FindAsync(id);
        if (product is null) return NotFound();
        product.Name = input.Name.Trim();
        product.Slug = await UniqueSlugAsync(input.Name, id);
        product.Description = input.Description.Trim();
        product.Price = input.Price;
        product.Stock = input.Stock;
        product.ImageUrl = input.ImageUrl;
        product.CategoryId = input.CategoryId;
        product.IsFeatured = input.IsFeatured;
        product.IsActive = input.IsActive;
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật sản phẩm.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await db.Products.AsNoTracking().Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
        return product is null ? NotFound() : View(product);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return NotFound();
        var ordered = await db.OrderItems.AnyAsync(x => x.ProductId == id);
        if (ordered)
        {
            product.IsActive = false;
            TempData["Success"] = "Sản phẩm đã có đơn hàng nên được chuyển sang trạng thái ngừng bán.";
        }
        else
        {
            db.Products.Remove(product);
            TempData["Success"] = "Đã xóa sản phẩm.";
        }
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCategoriesAsync(int? selected = null) =>
        ViewBag.Categories = new SelectList(await db.Categories.OrderBy(x => x.Name).ToListAsync(), "Id", "Name", selected);

    private async Task<string> UniqueSlugAsync(string name, int? excludedId = null)
    {
        var baseSlug = name.ToSlug();
        var slug = baseSlug;
        var index = 2;
        while (await db.Products.AnyAsync(x => x.Slug == slug && x.Id != excludedId)) slug = $"{baseSlug}-{index++}";
        return slug;
    }
}
