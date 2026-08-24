using System.ComponentModel.DataAnnotations;

namespace BlogManager.Models.Entities;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
    [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
    [Display(Name = "Tên danh mục")]
    public string Name { get; set; } = string.Empty;

    public ICollection<Post> Posts { get; set; } = [];
}
