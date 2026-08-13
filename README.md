# AI Study Hub - Hệ Thống Quản Lý và Hỗ trợ Học Tập Tích Hợp AI

**AI Study Hub** là nền tảng quản lý tài liệu học tập tập trung, đồng bộ hóa dữ liệu đa nền tảng và tích hợp Trợ lý AI Chatbot dựa trên nội dung tài liệu học tập. Dự án được xây dựng theo kiến trúc **API-First - Clean Architecture** trên nền tảng **.NET 10.0 SDK**.

---

## 1. Tác Nhân Hệ Thống

- **Guest:** Người dùng chưa xác thực, truy cập các trang giới thiệu hoặc Đăng ký/Đăng nhập.
- **Student / User:** Người dùng đã xác thực, quản lý tài liệu cá nhân, xem bản tóm tắt, ôn tập Flashcard và tương tác với AI Chatbot.
- **Admin:** Quản trị viên quản lý danh mục môn học, người dùng và giám sát hệ thống.
- **Chatbot Service:** Dịch vụ tích hợp AI xử lý ngôn ngữ tự nhiên, phân tích nội dung tài liệu để trả lời truy vấn.

---

## 2. Các Tính Năng Chính

### 2.1. Xác thực & Quản lý Tài khoản

- Đăng ký tài khoản.
- Đăng nhập & Xác thực hệ thống cấp phát JWT Token.
- Đăng xuất và vô hiệu hóa phiên làm việc.
- Quản lý và cập nhật thông tin cá nhân.
- Khôi phục / Quên mật khẩu qua Email.

### 2.2. Quản lý Tài liệu & Lưu trữ Đám mây

- **Upload:** Tải lên tài liệu học tập, ghi nhận metadata.
- **Categorization:** Phân loại tài liệu gắn liền với môn học.
- **Search & Filter:** Tìm kiếm theo từ khóa; lọc theo môn học và định dạng file.
- **CRUD Operations:** Xem chi tiết metadata, chỉnh sửa thông tin, tải xuống và xóa tài liệu.
- **Cloud Sync & Preview:** Đồng bộ hóa tập tin lên Cloud Storage, theo dõi trạng thái và hỗ trợ xem trước trực tuyến.

### 🤖 2.3. Trợ lý AI Chatbot

- **Document Querying:** Gửi câu hỏi và truy vấn thông tin trực tiếp dựa trên nội dung tài liệu.
- **Chat Context:** Tương tác trực tiếp với Trợ lý AI theo ngữ cảnh môn học/tài liệu.
- **Chat History:** Lưu trữ và hiển thị lịch sử các phiên hội thoại.

### 🎴 2.4. Công cụ Học tập AI

- **Auto Summarization:** Tự động phân tích và sinh bản tóm tắt tài liệu, gồm nội dung tóm lược và các điểm chính.
- **Flashcard Generation:** Tự động tạo bộ Flashcard từ tài liệu dựa trên AI Prompting.
- **Custom Flashcards:** Cho phép sinh viên tự tạo, chỉnh sửa hoặc bổ sung Flashcard thủ công.
- **Progress Tracking:** Theo dõi trạng thái đã thuộc/chưa thuộc của từng thẻ Flashcard để tối ưu hóa ôn tập.

---

## 3. Kiến Trúc Hệ Thống

Hệ thống sử dụng mô hình Client-Server tập trung, trong đó Web API đóng vai trò nguồn thông tin duy nhất:

```text
┌────────────────────────────────────────────────────────────────────────┐
│                        FRONTEND ECOSYSTEM                              │
│  ┌───────────────────┐    ┌───────────────────┐    ┌────────────────┐  │
│  │   React Web App   │    │  WPF Desktop App  │    │ .NET MAUI App  │  │
│  └─────────┬─────────┘    └─────────┬─────────┘    └───────┬────────┘  │
└────────────┼────────────────────────┼──────────────────────┼───────────┘
             │                        │                      │
             └──────────────────┐     │     ┌────────────────┘
                                ▼     ▼     ▼
                            (HTTP / RESTful API)
                                      │
┌─────────────────────────────────────┼──────────────────────────────────┐
│ BACKEND SERVICE (.NET 10.0 SDK)     ▼                                  │
│  ┌──────────────────────────────────────────────────────────────────┐  │
│  │                  PRN212.AIStudyHub.WebAPI                        │  │
│  │       (Controllers, JWT Authentication, Swagger, CORS)           │  │
│  └──────────────────────────────────┬───────────────────────────────┘  │
│                                     ▼                                  │
│  ┌──────────────────────────────────────────────────────────────────┐  │
│  │                PRN212.AIStudyHub.Application                     │  │
│  │       (Business Logic, Interfaces, DTOs, Use Cases)              │  │
│  └──────────────────┬───────────────────────────────┬───────────────┘  │
│                     │                               │                  │
│                     ▼                               ▼                  │
│  ┌──────────────────────────────────┐   ┌───────────────────────────┐  │
│  │   PRN212.AIStudyHub.Domain       │   │PRN212.AIStudyHub.Infra    │  │
│  │   (Core Entities, POCOs)         │◄──┤(EF Core, DBContext, Auth) │  │
│  └──────────────────────────────────┘   └───────────┬───────────────┘  │
└─────────────────────────────────────────────────────┼──────────────────┘
                                                      │
                  ┌───────────────────────────────────┼───────────────────────────────────┐
                  | Database                          |                                   |
                  |            ┌──────────────────────┴──────────────────────┐            |
                  |            ▼                                             ▼            |
                  |  ┌───────────────────┐                         ┌───────────────────┐  |
                  |  │    SQL Server     │                         │  Cloud Storage /  │  |
                  |  │    Database       │                         │  AI Chatbot Admin │  |
                  |  └───────────────────┘                         └───────────────────┘  |
                  |                                                                       |
                  └───────────────────────────────────────────────────────────────────────┘
```

### Cấu trúc dự án Monorepo:

- **`src/PRN212.AIStudyHub.Domain`**: Chứa các Core Entities và Domain Interfaces (Thuần POCO).
- **`src/PRN212.AIStudyHub.Application`**: Chứa Business Logic, DTOs, Service Interfaces.
- **`src/PRN212.AIStudyHub.Infrastructure`**: Chứa `AistudyHubDbContext`, Repositories, JWT Provider, BCrypt Hasher.
- **`src/PRN212.AIStudyHub.WebAPI`**: Tầng phân phối RESTful API, Middleware, Swagger UI.
- **`Prn212.AIStudyHub.WPF`**: Client ứng dụng Desktop cho Windows.
- **`PRN212.Web`**: Client Web phát triển bằng React + Vite.
- **`PRN212.Mobile`**: Client Ứng dụng Di động phát triển bằng .NET MAUI.

---

## 4. Hướng Dẫn Cài Đặt & Vận Hành

### 4.1. Chuẩn bị Hạ tầng & Database

1. **SQL Server Authentication** Kích hoạt tài khoản kết nối SQL Server
2. **Database:** Thực thi tệp tin `database/schema.sql` trên SQL Server.
3. **SQL Server Networking:** Kích hoạt TCP/IP trong SQL Server Configuration Manager (Cổng 1433).
4. **Connection String** Cấu hình chuỗi kết nối Database tại `src/PRN212.AIStudyHub.WebAPI/appsettings.json`

### 4.2. Khởi chạy WebAPI

```bash
dotnet run --project src/PRN212.AIStudyHub.WebAPI/PRN212.AIStudyHub.WebAPI.csproj
```

### 4.3. Khởi chạy các Client **_(Trong quá trình phát triển)_**

- **React Web Client**

  ```bash
  cd PRN212.Web
  npm install
  npm run dev
  ```

- **WPF Desktop Client**

  ```bash
  dotnet run --project Prn212.AIStudyHub.WPF/Prn212.AIStudyHub.WPF.csproj
  ```

  _Lưu ý: Cần phải cấu hình `Prn212.AIStudyHub.WPF/appsettings.json`_

- **.NET MAUI Mobile Client / React Native**

  _Chưa thiết lập_
