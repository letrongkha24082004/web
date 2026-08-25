using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManager.Data;
using ShopManager.Models.Entities;
using ShopManager.Models.ViewModels;
using ShopManager.Security;

namespace ShopManager.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class DashboardController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(new DashboardViewModel
    {
        ProductCount = await db.Products.CountAsync(),
        CategoryCount = await db.Categories.CountAsync(),
        TodayOrderCount = await db.Orders.CountAsync(x => x.CreatedAt >= DateTime.UtcNow.Date),
        LowStockCount = await db.Products.CountAsync(x => x.IsActive && x.Stock <= 10),
        OrderCount = await db.Orders.CountAsync(),
        PendingOrderCount = await db.Orders.CountAsync(x => x.Status == OrderStatus.Pending),
        Revenue = await db.Orders.Where(x => x.Status == OrderStatus.Completed).SumAsync(x => (decimal?)x.Total) ?? 0,
        RecentOrders = await db.Orders.AsNoTracking().OrderByDescending(x => x.CreatedAt).Take(6).ToListAsync()
    });
}
