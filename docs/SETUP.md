# Hướng dẫn setup

## Điều kiện tiên quyết

- Visual Studio 2022+ Community
- .NET 8.0 SDK
- SQL Server 2019+

## Các bước cài đặt

1. **Clone repository**
   - Nhập lệnh
     ```bash
     git clone https://github.com/nmt2103/PRN212-AI-Study-Hub.git
     cd Prn212.AIStudyHub
     ```

2. **Mở solution**
   - Mở Visual Studio
   - Chọn `Open a project or solution`
   - Tìm solution của repository đã clone `Prn212.AIStudyHub.slnx`

3. **Tạo database**
   - Mở SQL Server Management Studio
   - Chạy script database/schema.sql và database/seeding.sql

4. **Cập nhật connection string**
   - Tạo `appsettings.json` như thầy hướng dẫn

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "server=(local); database=StudyHubDB; uid=<YOUR_USERNAME>; pwd=<YOUR_PASSWORD>; TrustServerCertificate=True;"
     }
   }
   ```

5. **Tải / Khôi phục NuGet packages**
   - Nhập lệnh `dotnet restore`

6. **Build solution**
   - Nhập lệnh `dotnet build`

7. **Chạy application**
   - Nhấn F5 trong Visual Studio
   - Hoặc `dotnet run

## Troubleshooting

- **Connection string error**: Kiểm tra tên server trong SQL Server Management Studio
- **Package errors**: Chạy lại lệnh "dotnet restore"
- **Database not found**: Tạo database nếu chưa có.
