using System.ComponentModel.DataAnnotations;

namespace BlogManager_LeTrongKha.ViewModels;

public class PostCreateViewModel
{
    [Display(Name = "Tiêu đề")]
    [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
    [StringLength(200, MinimumLength = 3,
        ErrorMessage = "Tiêu đề phải có từ 3 đến 200 ký tự.")]
    [RegularExpression(@"^\s*\S[\s\S]+\S\s*$",
        ErrorMessage = "Tiêu đề phải có từ 3 đến 200 ký tự sau khi bỏ khoảng trắng thừa.")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Nội dung")]
    [Required(ErrorMessage = "Nội dung là bắt buộc.")]
    public string Content { get; set; } = string.Empty;

    [Display(Name = "Tác giả")]
    [Required(ErrorMessage = "Tác giả là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên tác giả không được vượt quá 100 ký tự.")]
    public string Author { get; set; } = "Lê Trọng Kha";

    [Display(Name = "Ngày đăng")]
    [DataType(DataType.Date)]
    public DateTime PublishedAt { get; set; } = DateTime.Today;

    [Display(Name = "Đã xuất bản")]
    public bool IsPublished { get; set; }

    [Display(Name = "Lượt xem")]
    [Range(0, int.MaxValue, ErrorMessage = "Lượt xem không được là số âm.")]
    public int ViewCount { get; set; }
}
