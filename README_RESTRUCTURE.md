# AIStudyHub - Bản 3-Layer (gọn) + Auth

3 project: DataAccess (Entities + DbContext) -> Services -> WPF.
Luồng: View -> Service -> DbContext -> SQL Server.

## Chạy lần đầu (Visual Studio 2022, Windows + SQL Server)
1. Mở `Prn212.AIStudyHub.slnx`.
2. Chạy `database/schema.sql` rồi `database/seeding.sql`.
3. Copy `Prn212.AIStudyHub.WPF/appsettings.example.json` -> `appsettings.json`, sửa chuỗi kết nối.
4. Restore NuGet -> Build (Ctrl+Shift+B) -> đặt WPF làm Startup -> F5.

## Luồng đăng nhập
App mở ở màn hình **Đăng nhập**. Đăng nhập thành công -> **Trang tài liệu** (danh sách + tìm/lọc/sắp xếp + tải lên + đăng xuất).

Tài khoản mẫu để test (từ seeding.sql):
- admin@aistudyhub.com / 123456
- thuannm@aistudyhub.com / 123456

(Mật khẩu mẫu lưu dạng thô nên vẫn đăng nhập được; tài khoản ĐĂNG KÝ MỚI được hash bằng PBKDF2.)

## Ai làm file nào
- AUTH 1: `Services/Auth/AuthService.cs` (Login/Register - ĐÃ LÀM), `Services/Auth/PasswordHasher.cs`, `WPF/Views/Auth/LoginWindow.*`, `WPF/Views/Auth/RegisterWindow.*`
- AUTH 2: `Services/Auth/AccountService.cs`, các màn hình profile/reset trong `WPF/Views/Auth/`
- DOC 1 : `Services/Documents/DocumentService.Commands.cs`, `WPF/Views/Documents/`
- DOC 2 : `Services/Documents/DocumentService.Queries.cs`, `WPF/Views/Documents/`

## File KHUNG CHUNG đã sửa trong lần này (cần cả nhóm biết)
- `WPF/App.xaml`         : StartupUri chuyển sang LoginWindow
- `WPF/App.xaml.cs`      : giữ session App.CurrentUser (bỏ user giả lập)
- `WPF/MainWindow.xaml(.cs)` : thiết kế lại thành Trang tài liệu sau đăng nhập
