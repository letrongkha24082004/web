using Microsoft.AspNetCore.Mvc;
using ShopManager.Services;

namespace ShopManager.Controllers;

public class CartController(ICartService cart) : Controller
{
    public async Task<IActionResult> Index() => View(await cart.GetAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1, string? returnUrl = null)
    {
        var added = await cart.AddAsync(productId, quantity);
        TempData[added ? "Success" : "Error"] =
            added ? "Đã thêm sản phẩm vào giỏ hàng." : "Sản phẩm không còn hàng.";
        return LocalRedirect(returnUrl ?? Url.Action(nameof(Index))!);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int productId, int quantity)
    {
        await cart.UpdateAsync(productId, quantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        cart.Remove(productId);
        return RedirectToAction(nameof(Index));
    }
}
