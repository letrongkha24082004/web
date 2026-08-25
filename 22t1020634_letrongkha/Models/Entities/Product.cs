using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopManager.Models.Entities;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
    [StringLength(150)]
    [Display(Name = "Tên sản phẩm")]
    public string Name { get; set; } = string.Empty;

    [StringLength(180)]
    public string Slug { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mô tả.")]
    [StringLength(2000)]
    [Display(Name = "Mô tả")]
    public string Description { get; set; } = string.Empty;

    [Range(1000, 1_000_000_000, ErrorMessage = "Giá phải từ 1.000 đồng.")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Giá bán")]
    public decimal Price { get; set; }

    [Range(0, 100000)]
    [Display(Name = "Tồn kho")]
    public int Stock { get; set; }

    [Url(ErrorMessage = "Đường dẫn ảnh không hợp lệ.")]
    [StringLength(500)]
    [Display(Name = "Ảnh sản phẩm")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Nổi bật")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Đang bán")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Ngày tạo")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Danh mục")]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
