using System.ComponentModel.DataAnnotations;

namespace ShopManager.Dtos;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public class ProductRequest
{
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;
    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;
    [Range(1000, 1_000_000_000)]
    public decimal Price { get; set; }
    [Range(0, 100000)]
    public int Stock { get; set; }
    [Url, StringLength(500)]
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
}
