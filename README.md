# AI Study Hub

Một ứng dụng quản lý tài liệu học tập toàn diện, giúp sinh viên và giáo viên tổ chức, chia sẻ, và quản lý tài liệu học một cách hiệu quả.

## 📋 Mục lục

- [Giới thiệu](#-giới-thiệu)
- [Các tính năng nổi bật](#-các-tính-năng-nổi-bật)
- [Tech Stack](#core)
- [Cài đặt](#-cài-đặt)
- [Cấu trúc dự án](#-cấu-trúc-dự-án)

---

## 🎯 Giới thiệu

**AI Study Hub** là một ứng dụng desktop được xây dựng để giúp sinh viên, giáo viên, và các cá nhân quản lý tài liệu học tập một cách có hệ thống.

### Tại sao cần ứng dụng này?

- 📚 Tổ chức tài liệu theo môn học một cách rõ ràng
- 🔐 Bảo mật tài khoản và quyền truy cập dữ liệu
- 🔍 Tìm kiếm nhanh chóng tài liệu bạn cần
- 📥 Upload, download, quản lý tài liệu dễ dàng
- 👥 Chia sẻ tài liệu giữa các người dùng
- 💾 Lưu trữ tập trung, không mất dữ liệu

---

## ✨ Các tính năng nổi bật

### 🔐 Quản lý tài khoản

- ✅ Đăng ký tài khoản mới
- ✅ Đăng nhập an toàn bằng email/password
- ✅ Quên mật khẩu - reset qua email
- ✅ Cập nhật thông tin profile
- ✅ Đăng xuất và session management

### 📄 Quản lý tài liệu

- ✅ Upload tài liệu (PDF, Word, Excel, PowerPoint, v.v.)
- ✅ Xem danh sách tài liệu với pagination
- ✅ Xem chi tiết và preview tài liệu
- ✅ Download tài liệu về máy tính
- ✅ Chỉnh sửa metadata tài liệu (tên, chủ đề, mô tả)
- ✅ Xóa tài liệu không cần thiết
- ✅ Tìm kiếm tài liệu theo tên, chủ đề
- ✅ Lọc tài liệu theo môn học
- ✅ Sắp xếp tài liệu theo ngày, tên, kích cỡ

### 🎓 Tính năng bổ sung

- ✅ Hỗ trợ nhiều loại file document
- ✅ Giao diện thân thiện, dễ sử dụng
- ✅ Hiệu năng cao, xử lý nhanh
- ✅ Báo lỗi chi tiết, thông báo rõ ràng

---

## 🛠️ Tech Stack

### Core

| Công nghệ                 | Phiên bản | Mục đích                |
| ------------------------- | --------- | ----------------------- |
| **.NET Core**             | 8.0       | Framework chính         |
| **C#**                    | Latest    | Ngôn ngữ lập trình      |
| **Entity Framework Core** | Latest    | ORM, tương tác database |

### Database

| Công nghệ      | Phiên bản | Mục đích          |
| -------------- | --------- | ----------------- |
| **SQL Server** | 2019+     | Database chính    |
| **Migrations** | EF Core   | Schema management |

### Desktop application

| Công nghệ        | Phiên bản    | Mục đích           |
| ---------------- | ------------ | ------------------ |
| **WPF**          | .NET Core 6+ | UI Framework       |
| **XAML**         | Latest       | UI markup language |
| **MVVM Pattern** | -            | ArchitectudyHub    |

---

## 📦 Cài đặt

### 1. Clone repository

```bash
git clone https://github.com/nmt2103/PRN212-AI-Study-Hub.git
cd PRN212-AI-Study-Hub
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Cấu hình database connection

- Copy file `appsettings.Example.json` → `appsettings.json`
- Cập nhật connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=<YOUR_SQL_SERVER>;database=StudyHubDB;uid=<YOUR_USER>;pwd=<YOUR_PASSWORD>;TrustServerCertificate=True;"
  }
}
```

### 4. Tạo database & apply migrations

```bash
dotnet ef dbcontext scaffold "Server=<YOUR_SQL_SERVER>;uid=<YOUR_USER>;pwd=<YOUR_PASSWORD;database=StudyHubDB;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --output-dir ./Entities
```

### 5. Build ứng dụng

```bash
dotnet build
```

### 6. Chạy ứng dụng

```bash
dotnet run --project Prn212.AIStudyHub
```

### 7. Chạy tests

```bash
dotnet test
```

## 📁 Cấu trúc dự án

```
AIStudyHub/
│
├── src/
│   ├── AIStudyHub.Core/       # Business logic, models, services
│   ├── AIStudyHub.Data/       # Database, EF Core, migrations
│   ├── AIStudyHub.UI/         # WPF views, viewmodels, styles
│   └── AIStudyHub/            # Entry point, configuration
│
├── tests/
│   ├── AIStudyHub.Core.Tests/
│   ├── AIStudyHub.Data.Tests/
│   └── AIStudyHub.UI.Tests/
│
├── .github/
│   ├── ISSUE_TEMPLATE/
│   └── workflows/
│
├── .gitignore
├── .gitattributes
├── README.md
├── CONTRIBUTING.md
└── AIStudyHub.slnx
```
