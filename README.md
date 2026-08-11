# AI Study Hub

Ứng dụng Desktop quản lý tài liệu học tập thông minh sử dụng WPF, ASP.NET Core Web API, Entity Framework Core (Database-First) và SQL Server.

---

## 1. Các Tính Năng Chính

- **Xác thực tài khoản (Auth):** Đăng nhập, đăng ký (mã hóa mật khẩu qua PBKDF2), đăng xuất và cập nhật hồ sơ cá nhân.
- **Quản lý tài liệu (Documents):** Dashboard danh sách tài liệu, tìm kiếm/lọc môn học/sắp xếp, tải lên (upload), xem chi tiết (detail view), tải xuống (download), chỉnh sửa thông tin và xóa tài liệu.

---

## 2. Kiến Trúc Dự Án (Clean Architecture + Web API)

Dự án áp dụng mô hình Clean Architecture kết hợp mô hình Client-Server:

```
WPF Client (HTTP) ──> Web API ──> Application ──> Domain / Infrastructure ──> SQL Server
```

- **Prn212.AIStudyHub.WPF:** Giao diện người dùng. Gọi API qua `HttpClient` thay vì kết nối DB trực tiếp.
- **src/PRN212.AIStudyHub.WebAPI:** Chứa các RESTful API Endpoints phục vụ Client.
- **src/PRN212.AIStudyHub.Application:** Xử lý logic nghiệp vụ (Business Logic), Interfaces, DTOs.
- **src/PRN212.AIStudyHub.Domain:** Chứa các Entities cốt lõi.
- **src/PRN212.AIStudyHub.Infrastructure:** Quản lý kết nối Database (`AistudyHubDbContext`) và Repositories.

---

## 3. Hướng Dẫn Cài Đặt & Chạy Nhanh

1. **Database:** Thực thi tệp tin [database/schema.sql](schema.sql) và [database/seeding.sql](seeding.sql) trên SQL Server.
2. **Cấu hình API:** Mở file `src/PRN212.AIStudyHub.WebAPI/appsettings.json` và cấu hình lại ConnectionString.
3. **Cấu hình Client:** Mở file `Prn212.AIStudyHub.WPF/appsettings.json` và cấu hình Base URL cho Web API.
4. **Khởi chạy API:**
   ```bash
   dotnet run --project src/PRN212.AIStudyHub.WebAPI/PRN212.AIStudyHub.WebAPI.csproj
   ```
5. **Khởi chạy WPF Client:**
   ```bash
   dotnet run --project Prn212.AIStudyHub.WPF/Prn212.AIStudyHub.WPF.csproj
   ```
