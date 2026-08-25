using BlogManager.Data;
using BlogManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogManager.Services;

public class CategoryService(ApplicationDbContext dbContext) : ICategoryService
{
    public async Task<CategoryPage> GetPageAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        var query = dbContext.Categories.AsNoTracking();
        var normalizedSearchTerm = searchTerm?.Trim();

        if (!string.IsNullOrWhiteSpace(normalizedSearchTerm))
        {
            var pattern = $"%{normalizedSearchTerm}%";
            query = query.Where(category => EF.Functions.Like(category.Name, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var safePageNumber = Math.Clamp(pageNumber, 1, totalPages);

        var categories = await query
            .OrderByDescending(category => category.Id)
            .Skip((safePageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new CategoryPage(categories, totalCount, safePageNumber);
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ThenBy(category => category.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Category?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Categories
            .AsNoTracking()
            .Include(category => category.Posts)
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public Task<bool> ExistsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Categories.AnyAsync(
            category => category.Id == id,
            cancellationToken);
    }

    public async Task<CategorySaveResult> CreateAsync(
        Category category,
        CancellationToken cancellationToken = default)
    {
        if (await NameExistsAsync(category.Name, null, cancellationToken))
        {
            return CategorySaveResult.DuplicateName;
        }

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return CategorySaveResult.Success;
    }

    public async Task<CategorySaveResult> UpdateAsync(
        Category category,
        CancellationToken cancellationToken = default)
    {
        var existingCategory = await dbContext.Categories
            .FirstOrDefaultAsync(item => item.Id == category.Id, cancellationToken);

        if (existingCategory is null)
        {
            return CategorySaveResult.NotFound;
        }

        if (await NameExistsAsync(category.Name, category.Id, cancellationToken))
        {
            return CategorySaveResult.DuplicateName;
        }

        existingCategory.Name = category.Name;
        await dbContext.SaveChangesAsync(cancellationToken);
        return CategorySaveResult.Success;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var category = await dbContext.Categories.FindAsync([id], cancellationToken);
        if (category is null)
        {
            return false;
        }

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private Task<bool> NameExistsAsync(
        string name,
        int? excludedId,
        CancellationToken cancellationToken)
    {
        return dbContext.Categories.AnyAsync(
            category =>
                category.Id != excludedId &&
                category.Name == name,
            cancellationToken);
    }
}
