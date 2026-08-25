using BlogManager.Models.Entities;
using BlogManager.Models.ViewModels;
using BlogManager.Security;
using BlogManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManager.Controllers;

public class TagsController(ITagService tagService) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index(
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var normalizedSearchTerm = searchTerm?.Trim();
        return View(new TagIndexViewModel
        {
            Tags = await tagService.GetAllAsync(normalizedSearchTerm, cancellationToken),
            SearchTerm = normalizedSearchTerm ?? string.Empty
        });
    }

    [Authorize(Roles = RoleNames.Admin)]
    public IActionResult Create() => View(new Tag());

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Tag tag, CancellationToken cancellationToken)
    {
        NormalizeAndValidate(tag);
        if (!ModelState.IsValid)
        {
            return View(tag);
        }

        if (await tagService.CreateAsync(tag, cancellationToken) == TagSaveResult.DuplicateName)
        {
            AddDuplicateNameError();
            return View(tag);
        }

        TempData["SuccessMessage"] = "Đã tạo thẻ.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var tag = await tagService.GetByIdAsync(id, cancellationToken);
        return tag is null ? NotFound() : View(tag);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Tag tag, CancellationToken cancellationToken)
    {
        if (id != tag.Id)
        {
            return BadRequest();
        }

        NormalizeAndValidate(tag);
        if (!ModelState.IsValid)
        {
            return View(tag);
        }

        var result = await tagService.UpdateAsync(tag, cancellationToken);
        if (result == TagSaveResult.NotFound)
        {
            return NotFound();
        }

        if (result == TagSaveResult.DuplicateName)
        {
            AddDuplicateNameError();
            return View(tag);
        }

        TempData["SuccessMessage"] = "Đã cập nhật thẻ.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var tag = await tagService.GetByIdAsync(id, cancellationToken);
        return tag is null ? NotFound() : View(tag);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = RoleNames.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        if (!await tagService.DeleteAsync(id, cancellationToken))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Đã xóa thẻ. Bài viết liên quan vẫn được giữ lại.";
        return RedirectToAction(nameof(Index));
    }

    private void NormalizeAndValidate(Tag tag)
    {
        tag.Name = tag.Name?.Trim() ?? string.Empty;
        ModelState.Clear();
        TryValidateModel(tag);
    }

    private void AddDuplicateNameError()
    {
        ModelState.AddModelError(nameof(Tag.Name), "Tên thẻ đã tồn tại.");
    }
}
