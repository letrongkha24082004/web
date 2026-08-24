using BlogManager.Data;
using BlogManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogManager.Services;

public class PostService(ApplicationDbContext dbContext) : IPostService
{
    public async Task<PostPage> GetPageAsync(
        string? search,
        string? sort,
        int? tagId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        var query = dbContext.Posts
            .Include(post => post.Category)
            .Include(post => post.Tags)
            .AsNoTracking()
            .AsQueryable();
        var normalizedSearch = search?.Trim();

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(post => post.Title.Contains(normalizedSearch));
        }

        if (tagId.HasValue)
        {
            query = query.Where(post => post.Tags.Any(tag => tag.Id == tagId.Value));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var safePageNumber = Math.Clamp(pageNumber, 1, Math.Max(1, totalPages));

        query = sort switch
        {
            "title" => query
                .OrderBy(post => post.Title)
                .ThenBy(post => post.Id),
            "oldest" => query
                .OrderBy(post => post.PublishedAt)
                .ThenBy(post => post.Id),
            "popular" => query
                .OrderByDescending(post => post.ViewCount)
                .ThenByDescending(post => post.PublishedAt)
                .ThenByDescending(post => post.Id),
            _ => query
                .OrderByDescending(post => post.PublishedAt)
                .ThenByDescending(post => post.Id)
        };

        var posts = await query
            .Skip((safePageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PostPage(posts, totalCount, safePageNumber, totalPages);
    }

    public Task<Post?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Posts
            .AsNoTracking()
            .Include(post => post.Category)
            .Include(post => post.Tags)
            .FirstOrDefaultAsync(post => post.Id == id, cancellationToken);
    }

    public async Task CreateAsync(
        Post post,
        IEnumerable<int> tagIds,
        CancellationToken cancellationToken = default)
    {
        var distinctTagIds = tagIds.Distinct().ToArray();
        if (distinctTagIds.Length > 0)
        {
            var tags = await dbContext.Tags
                .Where(tag => distinctTagIds.Contains(tag.Id))
                .ToListAsync(cancellationToken);
            foreach (var tag in tags)
            {
                post.Tags.Add(tag);
            }
        }

        dbContext.Posts.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(
        Post post,
        IEnumerable<int> tagIds,
        CancellationToken cancellationToken = default)
    {
        var existingPost = await dbContext.Posts
            .Include(item => item.Tags)
            .FirstOrDefaultAsync(item => item.Id == post.Id, cancellationToken);

        if (existingPost is null)
        {
            return false;
        }

        existingPost.Title = post.Title;
        existingPost.Content = post.Content;
        existingPost.Author = post.Author;
        existingPost.PublishedAt = post.PublishedAt;
        existingPost.IsPublished = post.IsPublished;
        existingPost.CategoryId = post.CategoryId;

        var distinctTagIds = tagIds.Distinct().ToArray();
        var selectedTags = distinctTagIds.Length == 0
            ? []
            : await dbContext.Tags
                .Where(tag => distinctTagIds.Contains(tag.Id))
                .ToListAsync(cancellationToken);
        existingPost.Tags.Clear();
        foreach (var tag in selectedTags)
        {
            existingPost.Tags.Add(tag);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var post = await dbContext.Posts.FindAsync([id], cancellationToken);
        if (post is null)
        {
            return false;
        }

        dbContext.Posts.Remove(post);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> IncrementViewCountAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var affectedRows = await dbContext.Posts
            .Where(post => post.Id == id)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    post => post.ViewCount,
                    post => post.ViewCount + 1),
                cancellationToken);
        return affectedRows == 1;
    }
}
