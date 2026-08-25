using System.ComponentModel.DataAnnotations;

namespace BlogManager.Dtos;

public sealed class PostDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public DateTime PublishedAt { get; init; }
    public bool IsPublished { get; init; }
    public int ViewCount { get; init; }
    public int? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public IReadOnlyList<TagDto> Tags { get; init; } = [];
}

public sealed record TagDto(int Id, string Name);

public sealed class PostSaveDto
{
    [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Tiêu đề phải có từ 3 đến 200 ký tự.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập nội dung.")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tác giả.")]
    [StringLength(100, ErrorMessage = "Tên tác giả không được vượt quá 100 ký tự.")]
    public string Author { get; set; } = string.Empty;

    public DateTime PublishedAt { get; set; } = DateTime.Today;

    public bool IsPublished { get; set; }

    public int? CategoryId { get; set; }

    public List<int> TagIds { get; set; } = [];
}

public sealed class PagedResponseDto<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
}
