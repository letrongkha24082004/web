using BlogManager.Models.Entities;

namespace BlogManager.Models.ViewModels;

public class CategoryIndexViewModel
{
    public IReadOnlyList<Category> Categories { get; init; } = [];

    public string SearchTerm { get; init; } = string.Empty;

    public int CurrentPage { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public int TotalItems { get; init; }

    public int TotalPages => TotalItems == 0
        ? 0
        : (int)Math.Ceiling(TotalItems / (double)PageSize);

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;
}
