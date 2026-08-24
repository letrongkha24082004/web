using BlogManager.Models.Entities;
using BlogManager.Models.ViewModels;
using BlogManager.Security;
using BlogManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogManager.Controllers;

public class PostsController(
    IPostService postService,
    ICategoryService categoryService,
    ITagService tagService) : Controller
{
    private const int PageSize = 5;

    [AllowAnonymous]
    public async Task<IActionResult> Index(
        string? search,
        string? sort,
        int? tagId,
        int? pageNumber,
        CancellationToken cancellationToken = default)
    {
        var normalizedSearch = search?.Trim();
        var normalizedSort = sort is "title" or "oldest" or "popular" ? sort : null;
        var result = await postService.GetPageAsync(
            normalizedSearch,
            normalizedSort,
            tagId,
            pageNumber ?? 1,
            PageSize,
            cancellationToken);

        return View(new PostListViewModel
        {
            Posts = result.Posts.ToList(),
            CurrentPage = result.PageNumber,
            TotalPages = result.TotalPages,
            Search = normalizedSearch,
            Sort = normalizedSort,
            TagId = tagId,
            AvailableTags = await tagService.GetAllAsync(cancellationToken: cancellationToken),
            PageSize = PageSize,
            TotalItems = result.TotalCount
        });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        if (!await postService.IncrementViewCountAsync(id, cancellationToken))
        {
            return NotFound();
        }

        var post = await postService.GetByIdAsync(id, cancellationToken);
        return post is null ? NotFound() : View(post);
    }

    [Authorize]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new PostFormViewModel
        {
            PublishedAt = DateTime.Today,
            Author = User.Identity?.Name ?? string.Empty
        };
        await PopulateOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        PostFormViewModel model,
        CancellationToken cancellationToken)
    {
        NormalizeAndValidate(model);
        await ValidateRelationsAsync(model, cancellationToken);
        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model, cancellationToken);
            return View(model);
        }

        await postService.CreateAsync(MapToEntity(model), model.SelectedTagIds, cancellationToken);
        TempData["SuccessMessage"] = "Đã tạo bài viết.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.CanEditPosts)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var post = await postService.GetByIdAsync(id, cancellationToken);
        if (post is null)
        {
            return NotFound();
        }

        var model = new PostFormViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Author = post.Author,
            PublishedAt = post.PublishedAt,
            IsPublished = post.IsPublished,
            CategoryId = post.CategoryId,
            SelectedTagIds = post.Tags.Select(tag => tag.Id).ToList()
        };
        await PopulateOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.CanEditPosts)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        PostFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        NormalizeAndValidate(model);
        await ValidateRelationsAsync(model, cancellationToken);
        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model, cancellationToken);
            return View(model);
        }

        if (!await postService.UpdateAsync(MapToEntity(model), model.SelectedTagIds, cancellationToken))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Đã cập nhật bài viết.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var post = await postService.GetByIdAsync(id, cancellationToken);
        return post is null ? NotFound() : View(post);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = RoleNames.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        int id,
        CancellationToken cancellationToken)
    {
        if (!await postService.DeleteAsync(id, cancellationToken))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Đã xóa bài viết.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateRelationsAsync(
        PostFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return;
        }

        if (model.CategoryId.HasValue &&
            !await categoryService.ExistsAsync(model.CategoryId.Value, cancellationToken))
        {
            ModelState.AddModelError(nameof(model.CategoryId), "Danh mục đã chọn không tồn tại.");
        }

        if (!await tagService.AllExistAsync(model.SelectedTagIds, cancellationToken))
        {
            ModelState.AddModelError(nameof(model.SelectedTagIds), "Một hoặc nhiều thẻ đã chọn không tồn tại.");
        }
    }

    private async Task PopulateOptionsAsync(
        PostFormViewModel model,
        CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetAllAsync(cancellationToken);
        model.CategoryOptions = categories
            .Select(category => new SelectListItem(category.Name, category.Id.ToString()))
            .ToList();

        var selectedTagIds = model.SelectedTagIds.ToHashSet();
        var tags = await tagService.GetAllAsync(cancellationToken: cancellationToken);
        model.TagOptions = tags
            .Select(tag => new SelectListItem(tag.Name, tag.Id.ToString(), selectedTagIds.Contains(tag.Id)))
            .ToList();
    }

    private void NormalizeAndValidate(PostFormViewModel model)
    {
        model.Title = model.Title?.Trim() ?? string.Empty;
        model.Author = model.Author?.Trim() ?? string.Empty;
        model.Content = model.Content?.Trim() ?? string.Empty;
        model.SelectedTagIds = model.SelectedTagIds.Distinct().ToList();
        ModelState.Clear();
        TryValidateModel(model);
    }

    private static Post MapToEntity(PostFormViewModel model)
    {
        return new Post
        {
            Id = model.Id,
            Title = model.Title,
            Content = model.Content,
            Author = model.Author,
            PublishedAt = model.PublishedAt,
            IsPublished = model.IsPublished,
            CategoryId = model.CategoryId
        };
    }
}
