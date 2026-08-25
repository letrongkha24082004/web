using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopManager.Extensions;
using ShopManager.Models.Entities;
using ShopManager.Security;

namespace ShopManager.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration)
    {
        await using var scope = services.CreateAsyncScope();
        var provider = scope.ServiceProvider;
        var db = provider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in RoleNames.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = configuration["SeedAdmin:Email"];
        var adminUserName = configuration["SeedAdmin:UserName"];
        var adminPassword = configuration["SeedAdmin:Password"];
        if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminUserName))
        {
            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var admin = await userManager.FindByNameAsync(adminUserName)
                ?? await userManager.FindByEmailAsync(adminEmail);
            if (admin is null)
            {
                if (string.IsNullOrWhiteSpace(adminPassword))
                {
                    throw new InvalidOperationException("Cần cấu hình SeedAdmin:Password để tạo tài khoản quản trị lần đầu.");
                }

                admin = new IdentityUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
                }
            }
            else if (!string.Equals(admin.UserName, adminUserName, StringComparison.Ordinal))
            {
                var userNameResult = await userManager.SetUserNameAsync(admin, adminUserName);
                if (!userNameResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join("; ", userNameResult.Errors.Select(x => x.Description)));
                }
            }

            if (!string.IsNullOrWhiteSpace(adminPassword) && !await userManager.CheckPasswordAsync(admin, adminPassword))
            {
                var resetToken = await userManager.GeneratePasswordResetTokenAsync(admin);
                var resetResult = await userManager.ResetPasswordAsync(admin, resetToken, adminPassword);
                if (!resetResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join("; ", resetResult.Errors.Select(x => x.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, RoleNames.Admin))
            {
                await userManager.AddToRoleAsync(admin, RoleNames.Admin);
            }
        }

        if (await db.Categories.AnyAsync())
        {
            return;
        }

        var categories = new[]
        {
            new Category { Name = "Thời trang", Description = "Trang phục trẻ trung cho mọi ngày" },
            new Category { Name = "Công nghệ", Description = "Phụ kiện và thiết bị tiện ích" },
            new Category { Name = "Đời sống", Description = "Sản phẩm làm đẹp không gian sống" },
            new Category { Name = "Thể thao", Description = "Năng động hơn trong từng chuyển động" }
        };
        foreach (var category in categories)
        {
            category.Slug = category.Name.ToSlug();
        }

        db.Categories.AddRange(categories);
        await db.SaveChangesAsync();

        db.Products.AddRange(
            CreateProduct("Áo khoác Urban Wind", "Thiết kế chống gió nhẹ, phom dáng hiện đại và dễ phối đồ.", 649000, 32, categories[0], true, "https://images.unsplash.com/photo-1551028719-00167b16eac5?auto=format&fit=crop&w=900&q=80"),
            CreateProduct("Sneaker Mono Street", "Đế êm, phối màu tối giản dành cho những ngày di chuyển nhiều.", 890000, 18, categories[0], true, "https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=900&q=80"),
            CreateProduct("Tai nghe AirBeat S2", "Âm thanh cân bằng, chống ồn và thời lượng pin lên đến 28 giờ.", 1290000, 24, categories[1], true, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=900&q=80"),
            CreateProduct("Bàn phím Mini 75", "Bàn phím cơ nhỏ gọn, kết nối ba chế độ và LED nền dịu mắt.", 1590000, 12, categories[1], false, "https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&w=900&q=80"),
            CreateProduct("Đèn bàn Halo", "Ánh sáng ấm, điều chỉnh ba mức và thiết kế tối giản cho góc làm việc.", 459000, 40, categories[2], true, "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?auto=format&fit=crop&w=900&q=80"),
            CreateProduct("Bình giữ nhiệt Flow", "Giữ nhiệt 12 giờ, dung tích 650ml, chất liệu thép không gỉ.", 329000, 60, categories[2], false, "https://images.unsplash.com/photo-1602143407151-7111542de6e8?auto=format&fit=crop&w=900&q=80"),
            CreateProduct("Thảm Yoga Balance", "Bề mặt chống trượt, dày 6mm, phù hợp luyện tập hàng ngày.", 399000, 27, categories[3], false, "https://images.unsplash.com/photo-1601925260368-ae2f83cf8b7f?auto=format&fit=crop&w=900&q=80"),
            CreateProduct("Túi thể thao Motion", "Ngăn chứa rộng, chống thấm nhẹ và có ngăn giày riêng biệt.", 549000, 21, categories[3], false, "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?auto=format&fit=crop&w=900&q=80")
        );
        await db.SaveChangesAsync();
    }

    private static Product CreateProduct(string name, string description, decimal price, int stock,
        Category category, bool featured, string imageUrl) => new()
    {
        Name = name,
        Slug = name.ToSlug(),
        Description = description,
        Price = price,
        Stock = stock,
        CategoryId = category.Id,
        IsFeatured = featured,
        IsActive = true,
        ImageUrl = imageUrl,
        CreatedAt = DateTime.UtcNow
    };
}
