using BlogManager.Models.Entities;

namespace BlogManager.Models.ViewModels;

public class PostListViewModel
{
    public List<Post> Posts { get; set; } = [];

    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; }

    public string? Search { get; set; }

    public string? Sort { get; set; }

    public int? TagId { get; set; }

    public IReadOnlyList<Tag> AvailableTags { get; set; } = [];

    public int PageSize { get; set; } = 5;

    public int TotalItems { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;
}
