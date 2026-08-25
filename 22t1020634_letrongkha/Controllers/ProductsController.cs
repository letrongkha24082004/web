using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManager.Data;

namespace ShopManager.Controllers;

public class ProductsController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Details(int id)
    {
        var product = await db.Products.AsNoTracking()
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        return product is null ? NotFound() : View(product);
    }
}
