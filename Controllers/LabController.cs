using BlogManager_LeTrongKha.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogManager_LeTrongKha.Controllers;

public class LabController : Controller
{
    public IActionResult Index()
    {
        var baiViet = new List<Post>
        {
            new() { Id = 1, Title = "C# cơ bản", Content = "Ôn tập cú pháp C# cần thiết.", Author = "Lê Trọng Kha", PublishedAt = new DateTime(2026, 7, 1), IsPublished = true, ViewCount = 120 },
            new() { Id = 2, Title = "MVC nhập môn", Content = "Tìm hiểu Model, View và Controller.", Author = "Lê Trọng Kha", PublishedAt = new DateTime(2026, 7, 3), IsPublished = false, ViewCount = 85 },
            new() { Id = 3, Title = "EF Core", Content = "Làm việc với dữ liệu qua ORM.", Author = "Lê Trọng Kha", PublishedAt = new DateTime(2026, 7, 5), IsPublished = true, ViewCount = 240 },
            new() { Id = 4, Title = "Razor View", Content = "Hiển thị dữ liệu trong tệp cshtml.", Author = "Lê Trọng Kha", PublishedAt = new DateTime(2026, 7, 7), IsPublished = true, ViewCount = 150 },
            new() { Id = 5, Title = "Routing ASP.NET Core", Content = "Ánh xạ URL đến controller action.", Author = "Lê Trọng Kha", PublishedAt = new DateTime(2026, 7, 9), IsPublished = false, ViewCount = 60 }
        };

        ViewBag.SoDaXuatBan = baiViet.Count(p => p.IsPublished);
        ViewBag.TieuDe = baiViet
            .Where(p => p.IsPublished)
            .OrderBy(p => p.Title)
            .Select(p => p.Title)
            .ToList();
        ViewBag.BaiDaXuatBan = baiViet
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.ViewCount)
            .ToList();
        ViewBag.TongLuotXem = baiViet.Sum(p => p.ViewCount);
        ViewBag.BaiNhieuLuotXemNhat = baiViet
            .OrderByDescending(p => p.ViewCount)
            .First();

        return View(baiViet);
    }
}
