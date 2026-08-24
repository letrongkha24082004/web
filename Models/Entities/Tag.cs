using System.ComponentModel.DataAnnotations;

namespace BlogManager.Models.Entities;

public class Tag
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên thẻ.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên thẻ phải có từ 2 đến 50 ký tự.")]
    [Display(Name = "Tên thẻ")]
    public string Name { get; set; } = string.Empty;

    public ICollection<Post> Posts { get; set; } = [];
}
