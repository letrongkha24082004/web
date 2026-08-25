# KHA Store - 22T1020634 Lê Trọng Kha

Website bán hàng xây dựng bằng ASP.NET Core MVC, Entity Framework Core,
SQLite và ASP.NET Core Identity.

## Chức năng khách hàng

- Xem, tìm kiếm, lọc danh mục, sắp xếp và phân trang sản phẩm.
- Xem chi tiết, thêm/cập nhật/xóa sản phẩm trong giỏ hàng.
- Đăng ký, đăng nhập, đặt hàng COD và theo dõi lịch sử đơn hàng.
- Kiểm tra tồn kho và trừ tồn kho trong transaction khi đặt hàng.

## Chức năng quản trị

- Dashboard thống kê sản phẩm, danh mục, đơn hàng và doanh thu.
- CRUD sản phẩm và danh mục với validation.
- Tìm kiếm, lọc và cập nhật trạng thái đơn hàng.
- REST API sản phẩm và Swagger tại `/swagger`.
- Toàn bộ khu vực `/Admin` chỉ dành cho vai trò `Admin`.

## Chạy project

Mật khẩu quản trị không được lưu trong mã nguồn. Đặt biến môi trường trước lần
chạy đầu để tạo tài khoản có tên đăng nhập `admin`:

```powershell
$env:SeedAdmin__Password = "<mat-khau-manh-cua-ban>"
dotnet tool restore
dotnet restore
dotnet ef database update
dotnet run
```

Mở `http://localhost:5300`. Mật khẩu phải có ít nhất 6 ký tự và có chữ số.
