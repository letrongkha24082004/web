using System;
using System.Collections.Generic;
using System.Linq;
using BlogManager.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BlogManager.Controllers
{
    public class LabController : Controller
    {
        public IActionResult Index()
        {
            var baiViet = new List<Post>
            {
                new Post { Id = 1, Title = "Nhập môn ASP.NET Core", Content = "Nội dung bài viết 1", Author = "An", PublishedAt = DateTime.Today.AddDays(-5), IsPublished = true, ViewCount = 120 },
                new Post { Id = 2, Title = "C# căn bản", Content = "Nội dung bài viết 2", Author = "Bình", PublishedAt = DateTime.Today.AddDays(-4), IsPublished = false, ViewCount = 45 },
                new Post { Id = 3, Title = "LINQ thực hành", Content = "Nội dung bài viết 3", Author = "An", PublishedAt = DateTime.Today.AddDays(-3), IsPublished = true, ViewCount = 210 },
                new Post { Id = 4, Title = "Razor View", Content = "Nội dung bài viết 4", Author = "Chi", PublishedAt = DateTime.Today.AddDays(-2), IsPublished = true, ViewCount = 88 },
                new Post { Id = 5, Title = "Entity Framework Core", Content = "Nội dung bài viết 5", Author = "Dũng", PublishedAt = DateTime.Today.AddDays(-1), IsPublished = true, ViewCount = 165 }
            };

            var daXuatBan = baiViet
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.ViewCount)
                .ToList();
            ViewBag.SoDaXuatBan = daXuatBan.Count;
            ViewBag.TongLuotXem = baiViet.Sum(p => p.ViewCount);
            ViewBag.BaiVietNhieuLuotXemNhat = baiViet.MaxBy(p => p.ViewCount);
            ViewBag.TacGia = baiViet.Select(p => p.Author).Distinct().OrderBy(name => name).ToList();

            return View(daXuatBan);
        }
    }
}
