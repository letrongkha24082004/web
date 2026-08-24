using System.ComponentModel.DataAnnotations;

namespace BlogManager.Models.Entities;

public class Post
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Tiêu đề phải có từ 3 đến 200 ký tự.")]
    [Display(Name = "Tiêu đề")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập nội dung.")]
    [Display(Name = "Nội dung")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tác giả.")]
    [StringLength(100, ErrorMessage = "Tên tác giả không được vượt quá 100 ký tự.")]
    [Display(Name = "Tác giả")]
    public string Author { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Ngày đăng")]
    public DateTime PublishedAt { get; set; } = DateTime.Today;

    [Display(Name = "Đã xuất bản")]
    public bool IsPublished { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Lượt xem không được là số âm.")]
    [Display(Name = "Lượt xem")]
    public int ViewCount { get; set; }

    [Display(Name = "Danh mục")]
    public int? CategoryId { get; set; }

    public Category? Category { get; set; }

    public ICollection<Tag> Tags { get; set; } = [];

    public string GetPopularityLabel() => ViewCount >= 100 ? "Phổ biến" : "Thường";
}
