using BlogManager_LeTrongKha.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogManager_LeTrongKha.Controllers;

public class PostsController : Controller
{
    private static readonly List<Post> Posts =
    [
        new() { Id = 1, Title = "C# cơ bản", Content = "Các kiến thức C# cần thiết để học ASP.NET Core.", Author = "Lê Trọng Kha", PublishedAt = new DateTime(2026, 7, 1), IsPublished = true, ViewCount = 120 },
        new() { Id = 2, Title = "MVC nhập môn", Content = "Tìm hiểu vai trò của Model, View và Controller.", Author = "Lê Trọng Kha", PublishedAt = new DateTime(2026, 7, 3), IsPublished = true, ViewCount = 85 },
        new() { Id = 3, Title = "EF Core", Content = "Làm việc với cơ sở dữ liệu bằng Entity Framework Core.", Author = "Lê Trọng Kha", PublishedAt = new DateTime(2026, 7, 5), IsPublished = false, ViewCount = 240 }
    ];

    public IActionResult Index()
    {
        return View(Posts);
    }

    public IActionResult Details(int id)
    {
        var post = Posts.SingleOrDefault(p => p.Id == id);

        if (post is null)
        {
            return NotFound();
        }

        return View(post);
    }
}
