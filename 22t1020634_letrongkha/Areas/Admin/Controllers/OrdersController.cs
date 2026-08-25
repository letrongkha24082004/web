using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManager.Data;
using ShopManager.Models.Entities;
using ShopManager.Security;

namespace ShopManager.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class OrdersController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index(OrderStatus? status, string? search)
    {
        var query = db.Orders.AsNoTracking().AsQueryable();
        if (status.HasValue) query = query.Where(x => x.Status == status);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.OrderCode.Contains(term) || x.CustomerEmail.Contains(term) || x.FullName.Contains(term));
        }
        ViewBag.Status = status;
        ViewBag.Search = search;
        return View(await query.OrderByDescending(x => x.CreatedAt).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await db.Orders.AsNoTracking().Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
        return order is null ? NotFound() : View(order);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        var order = await db.Orders.FindAsync(id);
        if (order is null) return NotFound();
        order.Status = status;
        await db.SaveChangesAsync();
        TempData["Success"] = $"Đơn {order.OrderCode} đã được cập nhật.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
