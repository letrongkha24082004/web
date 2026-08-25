using BlogManager.Models.Entities;

namespace BlogManager.Services;

public interface ICategoryService
{
    Task<CategoryPage> GetPageAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<CategorySaveResult> CreateAsync(Category category, CancellationToken cancellationToken = default);
    Task<CategorySaveResult> UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public sealed record CategoryPage(
    IReadOnlyList<Category> Categories,
    int TotalCount,
    int PageNumber);

public enum CategorySaveResult
{
    Success,
    NotFound,
    DuplicateName
}
