using BlogManager.Models.Entities;

namespace BlogManager.Services;

public interface ITagService
{
    Task<IReadOnlyList<Tag>> GetAllAsync(string? searchTerm = null, CancellationToken cancellationToken = default);
    Task<Tag?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> AllExistAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<TagSaveResult> CreateAsync(Tag tag, CancellationToken cancellationToken = default);
    Task<TagSaveResult> UpdateAsync(Tag tag, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public enum TagSaveResult
{
    Success,
    NotFound,
    DuplicateName
}
