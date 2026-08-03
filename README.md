# BlogManager_LeTrongKha

Project xuyên suốt bốn buổi đầu môn Lập trình Web.

- Sinh viên: Lê Trọng Kha
- MSSV: 22T1020634
- Framework: ASP.NET Core MVC — .NET 9
- Database: SQLite + Entity Framework Core 9.0.18

## Chạy ứng dụng

```powershell
dotnet tool restore
dotnet restore
dotnet tool run dotnet-ef database update
dotnet run
```

## Đường dẫn chính

- `/` — trang chủ và thông tin sinh viên
- `/Lab` — bài tập C# và LINQ của Buổi 2
- `/Posts` — danh sách bài viết của Buổi 3
- `/Posts/Details/1` — chi tiết bài viết
- `/Home/About` — giới thiệu
- `/Home/Contact` — liên hệ

Migrations trong `Migrations/` tạo lần lượt bảng `Posts`, bảng `Categories` và dữ liệu mẫu.
