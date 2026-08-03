using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlogManager_LeTrongKha.Migrations
{
    /// <inheritdoc />
    public partial class SeedPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Author", "Content", "IsPublished", "PublishedAt", "Title", "ViewCount" },
                values: new object[,]
                {
                    { 1, "Lê Trọng Kha", "Các kiến thức C# cần thiết để học ASP.NET Core.", true, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "C# cơ bản", 120 },
                    { 2, "Lê Trọng Kha", "Tìm hiểu vai trò của Model, View và Controller.", true, new DateTime(2026, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "MVC nhập môn", 85 },
                    { 3, "Lê Trọng Kha", "Làm việc với cơ sở dữ liệu bằng Entity Framework Core.", false, new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "EF Core", 240 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
