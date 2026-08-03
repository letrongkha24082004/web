using BlogManager_LeTrongKha.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogManager_LeTrongKha.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Post>().HasData(
            new Post
            {
                Id = 1,
                Title = "C# cơ bản",
                Content = "Các kiến thức C# cần thiết để học ASP.NET Core.",
                Author = "Lê Trọng Kha",
                PublishedAt = new DateTime(2026, 7, 1),
                IsPublished = true,
                ViewCount = 120
            },
            new Post
            {
                Id = 2,
                Title = "MVC nhập môn",
                Content = "Tìm hiểu vai trò của Model, View và Controller.",
                Author = "Lê Trọng Kha",
                PublishedAt = new DateTime(2026, 7, 3),
                IsPublished = true,
                ViewCount = 85
            },
            new Post
            {
                Id = 3,
                Title = "EF Core",
                Content = "Làm việc với cơ sở dữ liệu bằng Entity Framework Core.",
                Author = "Lê Trọng Kha",
                PublishedAt = new DateTime(2026, 7, 5),
                IsPublished = false,
                ViewCount = 240
            });
    }
}
