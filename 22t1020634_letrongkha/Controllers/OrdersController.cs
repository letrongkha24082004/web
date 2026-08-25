using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManager.Data;
using ShopManager.Models.Entities;
using ShopManager.Models.ViewModels;
using ShopManager.Services;

namespace ShopManager.Controllers;

[Authorize]
public class OrdersController(
    ApplicationDbContext db,
    ICartService cart,
    UserManager<IdentityUser> userManager) : Controller
{
    public async Task<IActionResult> Checkout()
    {
        var currentCart = await cart.GetAsync();
        if (currentCart.Items.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        return View(new CheckoutViewModel { Cart = currentCart });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        model.Cart = await cart.GetAsync();
        if (model.Cart.Items.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Giỏ hàng đang trống.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await using var transaction = await db.Database.BeginTransactionAsync();
        var ids = model.Cart.Items.Select(x => x.ProductId).ToArray();
        var products = await db.Products.Where(x => ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        foreach (var item in model.Cart.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product) || product.Stock < item.Quantity)
            {
                ModelState.AddModelError(string.Empty, $"Sản phẩm {item.Name} không đủ số lượng tồn kho.");
            }
        }

        if (!ModelState.IsValid)
        {
            await transaction.RollbackAsync();
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        var order = new Order
        {
            OrderCode = $"KHA{DateTime.Now:yyMMddHHmmss}",
            CustomerId = user!.Id,
            CustomerEmail = user.Email!,
            FullName = model.FullName,
            Phone = model.Phone,
            Address = model.Address,
            Note = model.Note,
            Total = model.Cart.Total + (model.Cart.Total >= 799000 ? 0 : 30000),
            Status = OrderStatus.Pending,
            Items = model.Cart.Items.Select(x => new OrderItem
            {
                ProductId = x.ProductId,
                ProductName = x.Name,
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity
            }).ToList()
        };

        foreach (var item in model.Cart.Items)
        {
            products[item.ProductId].Stock -= item.Quantity;
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        cart.Clear();
        return RedirectToAction(nameof(Success), new { id = order.Id });
    }

    public async Task<IActionResult> Success(int id)
    {
        var userId = userManager.GetUserId(User);
        var order = await db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == userId);
        return order is null ? NotFound() : View(order);
    }

    public async Task<IActionResult> MyOrders()
    {
        var userId = userManager.GetUserId(User);
        return View(await db.Orders.AsNoTracking()
            .Where(x => x.CustomerId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = userManager.GetUserId(User);
        var order = await db.Orders.AsNoTracking().Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == userId);
        return order is null ? NotFound() : View(order);
    }
}
