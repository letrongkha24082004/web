using BlogManager.Models.Entities;
using BlogManager.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogManager.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();

        await SeedRolesAndUsersAsync(serviceProvider, configuration);
        await SeedBlogDataAsync(dbContext);
    }

    private static async Task SeedRolesAndUsersAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        foreach (var roleName in RoleNames.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                EnsureSucceeded(roleResult, $"Không thể tạo vai trò {roleName}");
            }

            var email = configuration[$"SeedUsers:{roleName}:Email"];
            var password = configuration[$"SeedUsers:{roleName}:Password"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                continue;
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var userResult = await userManager.CreateAsync(user, password);
                EnsureSucceeded(userResult, $"Không thể tạo tài khoản mẫu {email}");
            }

            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                var addRoleResult = await userManager.AddToRoleAsync(user, roleName);
                EnsureSucceeded(addRoleResult, $"Không thể gán vai trò {roleName} cho {email}");
            }
        }
    }

    private static async Task SeedBlogDataAsync(ApplicationDbContext dbContext)
    {
        var categoryNames = new[] { "Lập trình", "Cơ sở dữ liệu", "Giao diện" };
        foreach (var categoryName in categoryNames)
        {
            if (!await dbContext.Categories.AnyAsync(category => category.Name == categoryName))
            {
                dbContext.Categories.Add(new Category { Name = categoryName });
            }
        }

        var tagNames = new[] { "ASP.NET Core", "C#", "EF Core", "SQLite", "Bootstrap" };
        foreach (var tagName in tagNames)
        {
            if (!await dbContext.Tags.AnyAsync(tag => tag.Name == tagName))
            {
                dbContext.Tags.Add(new Tag { Name = tagName });
            }
        }

        await dbContext.SaveChangesAsync();

        var categories = await dbContext.Categories
            .Where(category => categoryNames.Contains(category.Name))
            .ToDictionaryAsync(category => category.Name);
        var tags = await dbContext.Tags
            .Where(tag => tagNames.Contains(tag.Name))
            .ToDictionaryAsync(tag => tag.Name);

        var seeds = new[]
        {
            new PostSeed(
                "Bài viết đầu tiên",
                "Nội dung bài viết đầu tiên được lưu trong SQLite.",
                "Nguyễn Văn A",
                new DateTime(2024, 1, 15),
                true,
                42,
                "Lập trình",
                ["ASP.NET Core", "C#"]),
            new PostSeed(
                "Làm quen với ASP.NET Core MVC",
                "Tìm hiểu cách tổ chức controller, service, model và view.",
                "Trần Thị B",
                new DateTime(2024, 2, 20),
                true,
                128,
                "Lập trình",
                ["ASP.NET Core", "C#"]),
            new PostSeed(
                "Bản nháp về SQLite",
                "Bài viết này đang được biên tập.",
                "Lê Văn C",
                new DateTime(2024, 3, 10),
                false,
                17,
                "Cơ sở dữ liệu",
                ["SQLite"]),
            new PostSeed(
                "Thiết lập quan hệ với Entity Framework Core",
                "Khai báo khóa ngoại, navigation property và nạp dữ liệu bằng Include.",
                "Phạm Minh D",
                new DateTime(2024, 4, 5),
                true,
                96,
                "Cơ sở dữ liệu",
                ["EF Core", "SQLite"]),
            new PostSeed(
                "Tìm kiếm và sắp xếp bằng LINQ",
                "Xây dựng truy vấn IQueryable với Where và OrderBy.",
                "Hoàng Thu E",
                new DateTime(2024, 5, 18),
                true,
                73,
                "Lập trình",
                ["C#", "EF Core"]),
            new PostSeed(
                "Phân trang danh sách với Bootstrap",
                "Hiển thị thanh phân trang và giữ điều kiện tìm kiếm khi chuyển trang.",
                "Vũ Quốc F",
                new DateTime(2024, 6, 2),
                true,
                61,
                "Giao diện",
                ["Bootstrap", "ASP.NET Core"]),
            new PostSeed(
                "Hoàn thiện giao diện BlogManager",
                "Kết hợp bảng dữ liệu, biểu mẫu tìm kiếm và các nút thao tác.",
                "Đặng Mai G",
                new DateTime(2024, 7, 12),
                false,
                35,
                "Giao diện",
                ["Bootstrap"])
        };

        foreach (var seed in seeds)
        {
            var post = await dbContext.Posts
                .Include(item => item.Tags)
                .FirstOrDefaultAsync(item => item.Title == seed.Title);

            if (post is null)
            {
                post = new Post
                {
                    Title = seed.Title,
                    Content = seed.Content,
                    Author = seed.Author,
                    PublishedAt = seed.PublishedAt,
                    IsPublished = seed.IsPublished,
                    ViewCount = seed.ViewCount,
                    Category = categories[seed.CategoryName]
                };
                dbContext.Posts.Add(post);
            }
            else
            {
                post.CategoryId ??= categories[seed.CategoryName].Id;
                if (post.ViewCount == 0)
                {
                    post.ViewCount = seed.ViewCount;
                }
            }

            foreach (var tagName in seed.TagNames)
            {
                var tag = tags[tagName];
                if (post.Tags.All(item => item.Id != tag.Id))
                {
                    post.Tags.Add(tag);
                }
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static void EnsureSucceeded(IdentityResult result, string message)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"{message}: {string.Join("; ", result.Errors.Select(error => error.Description))}");
        }
    }

    private sealed record PostSeed(
        string Title,
        string Content,
        string Author,
        DateTime PublishedAt,
        bool IsPublished,
        int ViewCount,
        string CategoryName,
        string[] TagNames);
}
