using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManager.Data;
using ShopManager.Dtos;
using ShopManager.Extensions;
using ShopManager.Models.Entities;
using ShopManager.Security;

namespace ShopManager.Controllers.Api;

[ApiController]
[Route("api/products")]
public class ProductsApiController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<object>> GetAll(string? search, int? categoryId, int page = 1, int pageSize = 12)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var query = db.Products.AsNoTracking().Include(x => x.Category).Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.Contains(search.Trim()));
        }
        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId);
        }

        var total = await query.CountAsync();
        var products = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var data = products.Select(Map).ToList();
        return Ok(new { page, pageSize, total, data });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> Get(int id)
    {
        var product = await db.Products.AsNoTracking().Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
        return product is null ? NotFound() : Ok(Map(product));
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(ProductRequest request)
    {
        if (!await db.Categories.AnyAsync(x => x.Id == request.CategoryId))
        {
            ModelState.AddModelError(nameof(request.CategoryId), "Danh mục không tồn tại.");
            return ValidationProblem(ModelState);
        }

        var product = new Product
        {
            Name = request.Name.Trim(),
            Slug = await UniqueSlugAsync(request.Name),
            Description = request.Description.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            ImageUrl = request.ImageUrl,
            CategoryId = request.CategoryId
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();
        await db.Entry(product).Reference(x => x.Category).LoadAsync();
        return CreatedAtAction(nameof(Get), new { id = product.Id }, Map(product));
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductRequest request)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return NotFound();
        if (!await db.Categories.AnyAsync(x => x.Id == request.CategoryId)) return BadRequest("Danh mục không tồn tại.");
        product.Name = request.Name.Trim();
        product.Slug = await UniqueSlugAsync(request.Name, id);
        product.Description = request.Description.Trim();
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.ImageUrl = request.ImageUrl;
        product.CategoryId = request.CategoryId;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return NotFound();
        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<string> UniqueSlugAsync(string name, int? excludedId = null)
    {
        var baseSlug = name.ToSlug();
        var slug = baseSlug;
        var number = 2;
        while (await db.Products.AnyAsync(x => x.Slug == slug && x.Id != excludedId))
        {
            slug = $"{baseSlug}-{number++}";
        }
        return slug;
    }

    private static ProductDto Map(Product x) => new()
    {
        Id = x.Id,
        Name = x.Name,
        Description = x.Description,
        Price = x.Price,
        Stock = x.Stock,
        ImageUrl = x.ImageUrl,
        CategoryId = x.CategoryId,
        CategoryName = x.Category?.Name ?? string.Empty
    };
}
