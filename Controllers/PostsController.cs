using BlogManager_LeTrongKha.Data;
using BlogManager_LeTrongKha.Models;
using BlogManager_LeTrongKha.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogManager_LeTrongKha.Controllers;

public class PostsController : Controller
{
    private readonly ApplicationDbContext _context;

    public PostsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _context.Posts
            .AsNoTracking()
            .OrderByDescending(post => post.PublishedAt)
            .ToListAsync();

        return View(posts);
    }

    public async Task<IActionResult> Details(int id)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (post is null)
        {
            return NotFound();
        }

        return View(post);
    }

    public IActionResult Create()
    {
        return View(new PostCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PostCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var post = new Post
        {
            Title = model.Title.Trim(),
            Content = model.Content.Trim(),
            Author = model.Author.Trim(),
            PublishedAt = model.PublishedAt,
            IsPublished = model.IsPublished,
            ViewCount = model.ViewCount
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã thêm bài viết mới.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post is null)
        {
            return NotFound();
        }

        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,Title,Content,Author,PublishedAt,IsPublished,ViewCount")] Post post)
    {
        if (id != post.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(post);
        }

        var existingPost = await _context.Posts.FindAsync(id);

        if (existingPost is null)
        {
            return NotFound();
        }

        existingPost.Title = post.Title.Trim();
        existingPost.Content = post.Content.Trim();
        existingPost.Author = post.Author.Trim();
        existingPost.PublishedAt = post.PublishedAt;
        existingPost.IsPublished = post.IsPublished;
        existingPost.ViewCount = post.ViewCount;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await PostExistsAsync(post.Id))
            {
                return NotFound();
            }

            throw;
        }

        TempData["SuccessMessage"] = "Đã cập nhật bài viết.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (post is null)
        {
            return NotFound();
        }

        return View(post);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post is not null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa bài viết.";
        }

        return RedirectToAction(nameof(Index));
    }

    private Task<bool> PostExistsAsync(int id)
    {
        return _context.Posts.AnyAsync(post => post.Id == id);
    }
}
