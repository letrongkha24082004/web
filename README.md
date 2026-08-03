# BlogManager_LeTrongKha

Project xuyên suốt năm buổi đầu môn Lập trình Web.

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
- `/Posts` — danh sách và quản lý CRUD bài viết
- `/Posts/Create` — thêm bài viết với validation
- `/Posts/Details/1` — chi tiết bài viết
- `/Posts/Edit/1` — sửa bài viết
- `/Posts/Delete/1` — xác nhận xóa bài viết
- `/Home/About` — giới thiệu
- `/Home/Contact` — liên hệ

Buổi 5 chuyển `PostsController` sang EF Core bất đồng bộ, bổ sung đầy đủ Create/Read/Update/Delete, Data Annotations, validation phía server/client và `PostCreateViewModel`.

Migrations trong `Migrations/` tạo bảng `Posts`, bảng `Categories` và dữ liệu mẫu. Validation của `Post` được khai báo bằng Data Annotations và thực thi ở giao diện lẫn phía máy chủ; với SQLite, migration `AddPostValidation` chỉ ghi nhận metadata nên không cần thay đổi cấu trúc bảng.
