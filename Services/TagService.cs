using BlogManager.Data;
using BlogManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogManager.Services;

public class TagService(ApplicationDbContext dbContext) : ITagService
{
    public async Task<IReadOnlyList<Tag>> GetAllAsync(
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Tags
            .AsNoTracking()
            .Include(tag => tag.Posts)
            .AsQueryable();
        var normalizedSearchTerm = searchTerm?.Trim();

        if (!string.IsNullOrWhiteSpace(normalizedSearchTerm))
        {
            var pattern = $"%{normalizedSearchTerm}%";
            query = query.Where(tag => EF.Functions.Like(tag.Name, pattern));
        }

        return await query
            .OrderBy(tag => tag.Name)
            .ThenBy(tag => tag.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Tag?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Tags
            .AsNoTracking()
            .Include(tag => tag.Posts)
            .FirstOrDefaultAsync(tag => tag.Id == id, cancellationToken);
    }

    public async Task<bool> AllExistAsync(
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        var distinctIds = ids.Distinct().ToArray();
        if (distinctIds.Length == 0)
        {
            return true;
        }

        var existingCount = await dbContext.Tags.CountAsync(
            tag => distinctIds.Contains(tag.Id),
            cancellationToken);
        return existingCount == distinctIds.Length;
    }

    public async Task<TagSaveResult> CreateAsync(
        Tag tag,
        CancellationToken cancellationToken = default)
    {
        if (await NameExistsAsync(tag.Name, null, cancellationToken))
        {
            return TagSaveResult.DuplicateName;
        }

        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync(cancellationToken);
        return TagSaveResult.Success;
    }

    public async Task<TagSaveResult> UpdateAsync(
        Tag tag,
        CancellationToken cancellationToken = default)
    {
        var existingTag = await dbContext.Tags
            .FirstOrDefaultAsync(item => item.Id == tag.Id, cancellationToken);
        if (existingTag is null)
        {
            return TagSaveResult.NotFound;
        }

        if (await NameExistsAsync(tag.Name, tag.Id, cancellationToken))
        {
            return TagSaveResult.DuplicateName;
        }

        existingTag.Name = tag.Name;
        await dbContext.SaveChangesAsync(cancellationToken);
        return TagSaveResult.Success;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tag = await dbContext.Tags.FindAsync([id], cancellationToken);
        if (tag is null)
        {
            return false;
        }

        dbContext.Tags.Remove(tag);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private Task<bool> NameExistsAsync(
        string name,
        int? excludedId,
        CancellationToken cancellationToken)
    {
        return dbContext.Tags.AnyAsync(
            tag => tag.Id != excludedId && tag.Name == name,
            cancellationToken);
    }
}
