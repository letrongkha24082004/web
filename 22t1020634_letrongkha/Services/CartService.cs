using Microsoft.EntityFrameworkCore;
using ShopManager.Data;
using ShopManager.Extensions;
using ShopManager.Models.ViewModels;

namespace ShopManager.Services;

public class CartService(ApplicationDbContext db, IHttpContextAccessor accessor) : ICartService
{
    private const string CartKey = "ShoppingCart";
    private ISession Session => accessor.HttpContext?.Session
        ?? throw new InvalidOperationException("Không thể truy cập giỏ hàng.");

    public async Task<CartViewModel> GetAsync()
    {
        var lines = GetLines();
        if (lines.Count == 0)
        {
            return new CartViewModel();
        }

        var ids = lines.Select(x => x.ProductId).ToArray();
        var products = await db.Products.AsNoTracking()
            .Where(x => ids.Contains(x.Id) && x.IsActive)
            .ToDictionaryAsync(x => x.Id);
        var items = lines
            .Where(x => products.ContainsKey(x.ProductId))
            .Select(x =>
            {
                var product = products[x.ProductId];
                return new CartItemViewModel
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    UnitPrice = product.Price,
                    Quantity = Math.Min(x.Quantity, product.Stock),
                    Stock = product.Stock,
                    ImageUrl = product.ImageUrl
                };
            })
            .Where(x => x.Quantity > 0)
            .ToList();
        return new CartViewModel { Items = items };
    }

    public async Task<bool> AddAsync(int productId, int quantity = 1)
    {
        var product = await db.Products.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == productId && x.IsActive);
        if (product is null || product.Stock <= 0)
        {
            return false;
        }

        var lines = GetLines();
        var existing = lines.FirstOrDefault(x => x.ProductId == productId);
        var requested = Math.Clamp(quantity, 1, product.Stock);
        if (existing is null)
        {
            lines.Add(new CartLine(productId, requested));
        }
        else
        {
            lines[lines.IndexOf(existing)] = existing with
            {
                Quantity = Math.Min(existing.Quantity + requested, product.Stock)
            };
        }

        Save(lines);
        return true;
    }

    public async Task UpdateAsync(int productId, int quantity)
    {
        var lines = GetLines();
        var existing = lines.FirstOrDefault(x => x.ProductId == productId);
        if (existing is null)
        {
            return;
        }

        if (quantity <= 0)
        {
            lines.Remove(existing);
        }
        else
        {
            var stock = await db.Products.AsNoTracking()
                .Where(x => x.Id == productId)
                .Select(x => x.Stock)
                .FirstOrDefaultAsync();
            lines[lines.IndexOf(existing)] = existing with { Quantity = Math.Min(quantity, stock) };
        }

        Save(lines);
    }

    public void Remove(int productId)
    {
        var lines = GetLines();
        lines.RemoveAll(x => x.ProductId == productId);
        Save(lines);
    }

    public void Clear() => Session.Remove(CartKey);

    private List<CartLine> GetLines() => Session.GetObject<List<CartLine>>(CartKey) ?? [];
    private void Save(List<CartLine> lines) => Session.SetObject(CartKey, lines);
}
