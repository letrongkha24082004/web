# BlogManager

Ứng dụng xuyên suốt môn Lập trình Web, xây dựng bằng ASP.NET Core MVC, C#,
Entity Framework Core, SQLite, Identity và RESTful API.

## Chức năng đã hoàn thành

- CRUD bài viết, danh mục và thẻ; validation ở server và trình duyệt.
- Quan hệ một-nhiều `Category -> Posts` và nhiều-nhiều `Post <-> Tags`.
- Tìm kiếm theo tiêu đề, lọc theo thẻ, sắp xếp theo ngày/tiêu đề/lượt xem.
- Phân trang 5 bài mỗi trang và giữ nguyên bộ lọc khi chuyển trang.
- Đếm lượt xem khi mở trang chi tiết.
- Đăng ký, đăng nhập, đăng xuất bằng ASP.NET Core Identity.
- Ba vai trò `Admin`, `Editor`, `User` và phân quyền theo thao tác.
- RESTful API 5 endpoint, DTO, validation, tìm kiếm, lọc và phân trang.
- Swagger UI để xem tài liệu và thử API tại `/swagger`.
- Dữ liệu mẫu idempotent: 7 bài viết, 3 danh mục chuẩn và 5 thẻ.

## Phân quyền

| Thao tác | Khách | User | Editor | Admin |
| --- | ---: | ---: | ---: | ---: |
| Xem danh sách/chi tiết | Có | Có | Có | Có |
| Tạo bài viết | Không | Có | Có | Có |
| Sửa bài viết | Không | Có | Có | Có |
| Xóa bài viết | Không | Không | Không | Có |
| Quản lý danh mục/thẻ | Không | Không | Không | Có |

Email của tài khoản phát triển được cấu hình trong
`appsettings.Development.json`. Mật khẩu không được lưu trong mã nguồn; đặt các
biến môi trường sau trước lần chạy đầu tiên để ứng dụng tạo tài khoản mẫu:

```powershell
$env:SeedUsers__Admin__Password = "<mat-khau-admin>"
$env:SeedUsers__Editor__Password = "<mat-khau-editor>"
$env:SeedUsers__User__Password = "<mat-khau-user>"
dotnet run
```

Mỗi mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, chữ số và ký tự
đặc biệt. Không commit mật khẩu thật vào Git; tài khoản đăng ký mới được tự động
gán vai trò `User`.

## RESTful API

| Method | Endpoint | Quyền |
| --- | --- | --- |
| GET | `/api/posts` | Công khai |
| GET | `/api/posts/{id}` | Công khai |
| POST | `/api/posts` | User, Editor hoặc Admin |
| PUT | `/api/posts/{id}` | User, Editor hoặc Admin |
| DELETE | `/api/posts/{id}` | Admin |

`GET /api/posts` nhận các query parameter `search`, `sort`, `tagId`, `page` và
`pageSize`. Giá trị `sort` hỗ trợ `title`, `oldest`, `popular`; mặc định là mới
nhất. `pageSize` được giới hạn từ 1 đến 100.

## Chạy project

```powershell
dotnet tool restore
dotnet restore
dotnet ef database update
dotnet run
```

Ứng dụng mặc định chạy tại `http://localhost:5200` hoặc
`https://localhost:7200`. Database và khóa Data Protection nằm trong
`App_Data/`, được bỏ qua bởi Git. Migration và seed được chạy tự động khi ứng
dụng khởi động.

## Tài liệu bài giảng

Slide và PDF dùng để đối chiếu bài tập được lưu tập trung trong
[`docs/lectures`](docs/lectures/README.md), thay vì đặt rải rác ở thư mục gốc.

## Cấu trúc

```text
Areas/Identity/             # Trang đăng nhập và đăng ký
Controllers/                # MVC controllers
Controllers/Api/            # REST API controllers
Data/                       # DbContext, migration/role/data seeding
docs/lectures/              # Slide và PDF bài giảng
Dtos/                       # DTO đầu vào và đầu ra API
Migrations/                 # Lịch sử cấu trúc SQLite
Models/Entities/            # Post, Category, Tag
Models/ViewModels/          # Dữ liệu dành riêng cho Razor Views
Security/                   # Tên vai trò dùng thống nhất
Services/                   # Nghiệp vụ và truy cập EF Core
Views/                      # Razor Views và partial views
wwwroot/                    # CSS, biểu tượng và thư viện giao diện
```

Luồng MVC chính:

```text
Browser -> Controller -> Service -> ApplicationDbContext -> SQLite
```

Luồng API:

```text
Client -> /api/posts -> PostsApiController -> Service -> SQLite -> JSON DTO
```

## Migration mới nhất

`CompleteCourseFeatures` bổ sung Identity, vai trò, `ViewCount`, bảng `Tags`,
bảng nối `PostTags` và unique index không phân biệt hoa/thường cho tên danh
mục/thẻ. Khi nâng cấp database cũ, nên sao lưu file `.db` trước khi chạy
`dotnet ef database update`.
