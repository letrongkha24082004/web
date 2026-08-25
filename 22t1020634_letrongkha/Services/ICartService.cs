using ShopManager.Models.ViewModels;

namespace ShopManager.Services;

public interface ICartService
{
    Task<CartViewModel> GetAsync();
    Task<bool> AddAsync(int productId, int quantity = 1);
    Task UpdateAsync(int productId, int quantity);
    void Remove(int productId);
    void Clear();
}
