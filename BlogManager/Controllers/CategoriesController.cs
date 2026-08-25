using BlogManager.Models.Entities;
using BlogManager.Models.ViewModels;
using BlogManager.Security;
using BlogManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManager.Controllers;

public class CategoriesController(ICategoryService categoryService) : Controller
{
    private const int PageSize = 10;

    public async Task<IActionResult> Index(
        string? searchTerm,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var normalizedSearchTerm = searchTerm?.Trim();
        var result = await categoryService.GetPageAsync(
            normalizedSearchTerm,
            page,
            PageSize,
            cancellationToken);

        return View(new CategoryIndexViewModel
        {
            Categories = result.Categories,
            SearchTerm = normalizedSearchTerm ?? string.Empty,
            CurrentPage = result.PageNumber,
            PageSize = PageSize,
            TotalItems = result.TotalCount
        });
    }

    [Authorize(Roles = RoleNames.CanEditPosts)]
    public IActionResult Create()
    {
        return View(new Category());
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.CanEditPosts)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        Category category,
        CancellationToken cancellationToken)
    {
        NormalizeAndValidate(category);
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        var result = await categoryService.CreateAsync(category, cancellationToken);
        if (result == CategorySaveResult.DuplicateName)
        {
            AddDuplicateNameError();
            return View(category);
        }

        TempData["SuccessMessage"] = "Đã tạo danh mục.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.CanEditPosts)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var category = await categoryService.GetByIdAsync(id, cancellationToken);
        return category is null ? NotFound() : View(category);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.CanEditPosts)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        Category category,
        CancellationToken cancellationToken)
    {
        if (id != category.Id)
        {
            return BadRequest();
        }

        NormalizeAndValidate(category);
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        var result = await categoryService.UpdateAsync(category, cancellationToken);
        if (result == CategorySaveResult.NotFound)
        {
            return NotFound();
        }

        if (result == CategorySaveResult.DuplicateName)
        {
            AddDuplicateNameError();
            return View(category);
        }

        TempData["SuccessMessage"] = "Đã cập nhật danh mục.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.CanEditPosts)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var category = await categoryService.GetByIdAsync(id, cancellationToken);
        return category is null ? NotFound() : View(category);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = RoleNames.CanEditPosts)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        int id,
        CancellationToken cancellationToken)
    {
        if (!await categoryService.DeleteAsync(id, cancellationToken))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Đã xóa danh mục. Các bài viết liên quan đã chuyển thành chưa phân loại.";
        return RedirectToAction(nameof(Index));
    }

    private void NormalizeAndValidate(Category category)
    {
        category.Name = category.Name?.Trim() ?? string.Empty;
        ModelState.Clear();
        TryValidateModel(category);
    }

    private void AddDuplicateNameError()
    {
        ModelState.AddModelError(nameof(Category.Name), "Tên danh mục đã tồn tại.");
    }
}
