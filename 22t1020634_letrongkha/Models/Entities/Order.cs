using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopManager.Models.Entities;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipping,
    Completed,
    Cancelled
}

public class Order
{
    public int Id { get; set; }

    [StringLength(30)]
    [Display(Name = "Mã đơn")]
    public string OrderCode { get; set; } = string.Empty;

    [Required]
    public string CustomerId { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required, Phone, StringLength(20)]
    [Display(Name = "Số điện thoại")]
    public string Phone { get; set; } = string.Empty;

    [Required, StringLength(300)]
    [Display(Name = "Địa chỉ nhận hàng")]
    public string Address { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? Note { get; set; }

    [Display(Name = "Trạng thái")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Tổng tiền")]
    public decimal Total { get; set; }

    [Display(Name = "Ngày đặt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
