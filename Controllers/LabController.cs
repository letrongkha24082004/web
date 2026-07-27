using System;
using System.Collections.Generic;
using System.Linq;
using BlogManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogManager.Controllers
{
    public class LabController : Controller
    {
        public IActionResult Index()
        {
            var baiViet = new List<Post>
            {
                new Post { Id = 1, Title = "Bài viết 1", Content = "Nội dung bài viết 1", PublishedAt = DateTime.Now, IsPublished = true },
                new Post { Id = 2, Title = "Bài viết 2", Content = "Nội dung bài viết 2", PublishedAt = DateTime.Now, IsPublished = false },
                new Post { Id = 3, Title = "Bài viết 3", Content = "Nội dung bài viết 3", PublishedAt = DateTime.Now, IsPublished = true }
            };

            ViewBag.SoDaXuatBan = baiViet.Count(p => p.IsPublished);
            ViewBag.TieuDe = baiViet
                .Where(p => p.IsPublished)
                .OrderBy(p => p.Title)
                .Select(p => p.Title)
                .ToList();

            return View(baiViet);
        }
    }
}