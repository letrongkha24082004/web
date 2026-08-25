# ASP.NET Core Web Projects

Repository bài tập môn Lập trình Web của **22T1020634 - Lê Trọng Kha**.

## Danh sách project

| Project | Nội dung | Cổng mặc định | Tài liệu |
| --- | --- | ---: | --- |
| `BlogManager/BlogManager.csproj` | Blog MVC theo các chương bài giảng, Identity và REST API | `5200` | [Chi tiết](docs/BlogManager.md) |
| `22t1020634_letrongkha/ShopManager.csproj` | Website bán hàng và hệ thống quản trị chuyên nghiệp | `5300` | [Chi tiết](22t1020634_letrongkha/README.md) |

## Cấu trúc repository

```text
.
├── WebProjects.slnx              # Solution mở và build cả hai project
├── BlogManager/                  # Project bài giảng BlogManager độc lập
│   ├── BlogManager.csproj
│   └── Areas, Controllers, Models, Views
├── 22t1020634_letrongkha/        # Project website bán hàng độc lập
│   ├── Areas/Admin/              # Khu vực quản trị
│   ├── Areas/Identity/           # Đăng nhập và đăng ký
│   ├── Controllers/Api/          # REST API sản phẩm
│   ├── Data, Models, Services/   # Dữ liệu và nghiệp vụ
│   ├── Views/                    # Giao diện khách hàng
│   └── wwwroot/                  # CSS và thư viện giao diện cần thiết
├── docs/BlogManager.md           # Tài liệu project BlogManager
└── docs/lectures/                # Slide và PDF bài giảng
```

## Build toàn bộ repository

```powershell
dotnet tool restore
dotnet restore WebProjects.slnx
dotnet build WebProjects.slnx
```

## Chạy từng project

```powershell
dotnet run --project BlogManager/BlogManager.csproj
dotnet run --project 22t1020634_letrongkha/ShopManager.csproj
```

Mật khẩu phát triển, SQLite database, Data Protection keys, `bin/` và `obj/`
đều được loại khỏi Git. Mỗi project có README riêng hướng dẫn cấu hình tài khoản mẫu.
