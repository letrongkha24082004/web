using BlogManager.Models.Entities;

namespace BlogManager.Models.ViewModels;

public class TagIndexViewModel
{
    public IReadOnlyList<Tag> Tags { get; init; } = [];

    public string SearchTerm { get; init; } = string.Empty;
}
