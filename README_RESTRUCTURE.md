# AIStudyHub - Bản 3-Layer (gọn)

3 project:
- `Prn212.AIStudyHub.DataAccess` - Entities + DbContext (EF Core ở đây)
- `Prn212.AIStudyHub.Services`   - Services (nghiệp vụ)
- `Prn212.AIStudyHub.WPF`        - Giao diện (project chạy)

Luồng: View -> Service -> DbContext -> SQL Server. Không có tầng Repository,
không DI container, không interface -> ít code, dễ hiểu.

## Chạy lần đầu (Visual Studio 2022, máy Windows có SQL Server)
1. Mở `Prn212.AIStudyHub.slnx`.
2. Chạy `database/schema.sql` rồi `database/seeding.sql` để tạo DB + dữ liệu mẫu.
3. Copy `Prn212.AIStudyHub.WPF/appsettings.example.json` thành `appsettings.json`, sửa chuỗi kết nối.
4. Chuột phải Solution -> Restore NuGet Packages -> Build (Ctrl+Shift+B).
5. Đặt WPF làm Startup Project -> F5.

## Ai làm file nào (làm song song, không đụng nhau)
- AUTH 1: `Services/Auth/AuthService.cs`  + `WPF/Views/Auth/` (login, register, session)
- AUTH 2: `Services/Auth/AccountService.cs` + `WPF/Views/Auth/` (profile, reset mật khẩu)
- DOC 1 : `Services/Documents/DocumentService.Commands.cs` + `WPF/Views/Documents/` (upload, sửa, xóa)
- DOC 2 : `Services/Documents/DocumentService.Queries.cs`  + `WPF/Views/Documents/` (list, tìm, lọc, sắp xếp)

DocumentService là MỘT class chia làm 2 file (từ khóa `partial`) nên 2 người Doc
mỗi người một file, không sửa trùng.
