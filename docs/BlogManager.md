# BlogManager

Project thực hành xuyên suốt bài giảng ASP.NET Core MVC.

## Chức năng

- CRUD bài viết, danh mục và thẻ với validation.
- Quan hệ `Category -> Posts` và `Post <-> Tags`.
- Tìm kiếm, lọc, sắp xếp, phân trang và đếm lượt xem.
- Đăng ký, đăng nhập và phân quyền `Admin`, `Editor`, `User`.
- REST API bài viết và Swagger tại `/swagger`.
- SQLite, Entity Framework Core, migration và dữ liệu mẫu.

## Chạy project

```powershell
$env:SeedUsers__Admin__Password = "<mat-khau-admin>"
$env:SeedUsers__Editor__Password = "<mat-khau-editor>"
$env:SeedUsers__User__Password = "<mat-khau-user>"
dotnet tool restore
dotnet restore BlogManager/BlogManager.csproj
dotnet run --project BlogManager/BlogManager.csproj
```

Ứng dụng mặc định chạy tại `http://localhost:5200`.

## Phân quyền

| Thao tác | Khách | User | Editor | Admin |
| --- | ---: | ---: | ---: | ---: |
| Xem nội dung | Có | Có | Có | Có |
| Tạo và sửa bài viết | Không | Có | Có | Có |
| Xóa bài viết | Không | Không | Không | Có |
| Quản lý danh mục và thẻ | Không | Không | Không | Có |

## API

| Method | Endpoint | Quyền |
| --- | --- | --- |
| GET | `/api/posts` | Công khai |
| GET | `/api/posts/{id}` | Công khai |
| POST | `/api/posts` | User, Editor, Admin |
| PUT | `/api/posts/{id}` | User, Editor, Admin |
| DELETE | `/api/posts/{id}` | Admin |

Database và khóa Data Protection nằm trong `BlogManager/App_Data/` và không được đưa lên Git.
