using BlogManager.Models.Entities;

namespace BlogManager.Services;

public interface IPostService
{
    Task<PostPage> GetPageAsync(
        string? search,
        string? sort,
        int? tagId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<Post?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAsync(Post post, IEnumerable<int> tagIds, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Post post, IEnumerable<int> tagIds, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> IncrementViewCountAsync(int id, CancellationToken cancellationToken = default);
}

public sealed record PostPage(
    IReadOnlyList<Post> Posts,
    int TotalCount,
    int PageNumber,
    int TotalPages);
