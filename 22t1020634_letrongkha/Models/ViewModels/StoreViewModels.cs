using System.ComponentModel.DataAnnotations;
using ShopManager.Models.Entities;

namespace ShopManager.Models.ViewModels;

public class StoreIndexViewModel
{
    public IReadOnlyList<Product> Products { get; init; } = [];
    public IReadOnlyList<Category> Categories { get; init; } = [];
    public string? Search { get; init; }
    public int? CategoryId { get; init; }
    public string Sort { get; init; } = "newest";
    public int Page { get; init; }
    public int TotalPages { get; init; }
}

public record CartLine(int ProductId, int Quantity);

public class CartItemViewModel
{
    public int ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public int Stock { get; init; }
    public string? ImageUrl { get; init; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public class CartViewModel
{
    public IReadOnlyList<CartItemViewModel> Items { get; init; } = [];
    public decimal Total => Items.Sum(x => x.LineTotal);
    public int Count => Items.Sum(x => x.Quantity);
}

public class CheckoutViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên."), StringLength(100)]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại."), Phone, StringLength(20)]
    [Display(Name = "Số điện thoại")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng."), StringLength(300)]
    [Display(Name = "Địa chỉ nhận hàng")]
    public string Address { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? Note { get; set; }

    public CartViewModel Cart { get; set; } = new();
}

public class DashboardViewModel
{
    public int ProductCount { get; init; }
    public int CategoryCount { get; init; }
    public int TodayOrderCount { get; init; }
    public int LowStockCount { get; init; }
    public int PendingOrderCount { get; init; }
    public int OrderCount { get; init; }
    public decimal Revenue { get; init; }
    public IReadOnlyList<Order> RecentOrders { get; init; } = [];
}
