# AI Study Hub

Ứng dụng Desktop quản lý tài liệu học tập thông minh sử dụng WPF (.NET 8.0), Entity Framework Core (Database-First) và SQL Server.

---

## 1. Các Tính Năng Chính

- **Xác thực tài khoản (Auth):** Đăng nhập, đăng ký (mã hóa mật khẩu qua PBKDF2), đăng xuất và cập nhật hồ sơ cá nhân.
- **Quản lý tài liệu (Documents):** Dashboard danh sách tài liệu, tìm kiếm/lọc môn học/sắp xếp, tải lên (upload), xem chi tiết (detail view), tải xuống (download), chỉnh sửa thông tin và xóa tài liệu.

---

## 2. Kiến Trúc Dự Án (3-Layer)

Dự án áp dụng mô hình 3-Layer rút gọn với luồng dữ liệu một chiều:

```
Presentation (WPF) ──> Business (Services) ──> DataAccess (DbContext) ──> SQL Server
```

- **Prn212.AIStudyHub.DataAccess:** Quản lý kết nối Database (DbContext) và các thực thể (Entities).
- **Prn212.AIStudyHub.Services:** Xử lý toàn bộ logic nghiệp vụ, chia theo thư mục tính năng (`Auth` và `Documents`).
- **Prn212.AIStudyHub.WPF:** Giao diện người dùng (Views, Resource Styles) và quản lý session (`App.xaml.cs`).

---

## 3. Hướng Dẫn Cài Đặt & Chạy Nhanh

1. **Database:** Thực thi tệp tin [database/schema.sql](schema.sql) và [database/seeding.sql](seeding.sql) trên SQL Server.
2. **Cấu hình:** Sao chép `Prn212.AIStudyHub.WPF/appsettings.example.json` thành `appsettings.json` và cấu hình lại ConnectionString.
3. **Khởi chạy:**
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project Prn212.AIStudyHub.WPF
   ```
