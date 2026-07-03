# AIStudyHub — Tài liệu bàn giao kiến trúc 3-Layer (bản gọn)

> Tài liệu dành cho **4 thành viên** trong nhóm. Đọc kỹ trước khi bắt đầu code.
> Mục tiêu: mỗi người làm ở file/folder riêng, hạn chế tối đa conflict khi push lên GitHub.

---

## 1. Tài liệu này để làm gì

Trước đây toàn bộ code nằm trong **một project WPF duy nhất**, entity và `DbContext` để lẫn nhau, và phần Upload gọi thẳng database ngay trong code-behind của giao diện. Cách này khiến 4 người dễ sửa trùng file → conflict liên tục.

Bản này chia lại thành **3-layer gọn**: đủ tách bạch để làm việc song song, nhưng lược bỏ những phần rườm rà không cần thiết cho một đồ án (không có tầng Repository riêng, không dùng DI container, không cần interface). Tài liệu giải thích *từng project, từng file làm gì và tại sao*, kèm **hướng dẫn cụ thể cho từng người** cần làm gì, ở đâu, theo thứ tự nào.

---

## 2. Kiến trúc 3-Layer (bản gọn) là gì và tại sao chọn nó

Ứng dụng chia thành 3 tầng, mỗi tầng là một project:

| Tầng | Project | Trách nhiệm |
|---|---|---|
| Presentation (giao diện) | `Prn212.AIStudyHub.WPF` | Hiển thị, nhận thao tác người dùng. **Không** đụng database. |
| Business (nghiệp vụ) | `Prn212.AIStudyHub.Services` | Logic: kiểm tra dữ liệu, mã hóa mật khẩu, xử lý file, truy vấn dữ liệu. |
| Data (dữ liệu) | `Prn212.AIStudyHub.DataAccess` | Các lớp dữ liệu (`AppUser`, `Document`, `Subject`) và `AistudyHubDbContext`. |

**Quy tắc vàng — phải nhớ:** tầng trên được gọi tầng dưới, **không bao giờ ngược lại**.

```
View (WPF)  →  Service  →  AistudyHubDbContext  →  SQL Server
```

Ví dụ đúng: khi bấm "Upload", `UploadDocumentWindow` gọi `documentService.UploadAsync(...)`, service này mở `AistudyHubDbContext` để lưu.

Ví dụ **sai** (tuyệt đối tránh): giao diện tự viết `new AistudyHubDbContext()` rồi `.SaveChanges()`.

### Vì sao bản này gọn hơn

So với kiểu 3-layer "đầy đủ", bản này lược bỏ 3 thứ để giảm số lượng file và khái niệm:

- **Không có tầng Repository riêng.** Service làm việc thẳng với `DbContext`. (Đỡ một tầng interface + class trung gian.)
- **Không dùng DI container.** Giữ nguyên `StartupUri` như mặc định của WPF; giao diện tự tạo service bằng `new DocumentService()`. (Không phải cấu hình đăng ký dịch vụ.)
- **Không dùng interface.** Dùng thẳng class. (Ít file, dễ đọc cho người mới.)

**Lợi ích cho nhóm vẫn được giữ nguyên:**
- **Chống conflict:** yếu tố quyết định là *chia folder theo tính năng + mỗi người một file*, không phải DI hay số project.
- **Không phải chờ nhau:** mỗi người làm trọn tính năng của mình theo chiều dọc (giao diện → service → database).
- **Dễ test, dễ sửa:** logic tách khỏi giao diện, lỗi ở đâu tìm đúng tầng đó.

---

## 3. Sơ đồ tổng thể

```
Prn212.AIStudyHub.slnx
│
├── Prn212.AIStudyHub.DataAccess/        # Tầng dữ liệu
│   ├── Entities/
│   │   ├── AppUser.cs
│   │   ├── Document.cs
│   │   └── Subject.cs
│   └── AistudyHubDbContext.cs           # File chung — chỉ 1 người quản
│
├── Prn212.AIStudyHub.Services/          # Tầng nghiệp vụ
│   ├── Auth/
│   │   ├── AuthService.cs               # ⟵ AUTH 1 (login / register / session)
│   │   └── AccountService.cs            # ⟵ AUTH 2 (profile / reset mật khẩu)
│   └── Documents/
│       ├── DocumentService.Commands.cs  # ⟵ DOC 1 (upload / sửa / xóa)
│       └── DocumentService.Queries.cs   # ⟵ DOC 2 (list / tìm / lọc / sắp xếp)
│
└── Prn212.AIStudyHub.WPF/               # Tầng giao diện (project chạy)
    ├── Views/
    │   ├── Auth/                        # ⟵ Nhóm AUTH
    │   └── Documents/                   # ⟵ Nhóm DOCUMENT (UploadDocumentWindow ở đây)
    ├── Resources/                       # Styles dùng chung
    ├── App.xaml / App.xaml.cs           # StartupUri + session
    ├── MainWindow.xaml                  # màn hình chính (điều hướng) — vùng chung
    └── appsettings.json                 # chuỗi kết nối database
```

Nguyên tắc chia folder: **tách theo TÍNH NĂNG (Auth / Document), không tách theo TẦNG.** Nhóm Auth chỉ đụng các folder `Auth/`, nhóm Document chỉ đụng `Documents/`.

---

## 4. Giải thích chi tiết từng project

### 4.1. `Prn212.AIStudyHub.DataAccess` — Tầng dữ liệu

Chứa các lớp dữ liệu và kết nối database. Gói Entity Framework Core nằm ở đây.

| File | Ý nghĩa |
|---|---|
| `Entities/AppUser.cs` | Một tài khoản: `Email`, `PasswordHash`, `FirstName`, `LastName`, `Role`, `IsActive`, `CreatedAt`... |
| `Entities/Document.cs` | Một tài liệu: `Title`, `FileName`, `StoragePath`, `FileSize`, `FileExtension`, `ContentType`, `UploadedAt`, khóa ngoại `UserId`, `SubjectId`. |
| `Entities/Subject.cs` | Một môn học: `Name`, `Description`. |
| `AistudyHubDbContext.cs` | Đại diện kết nối database (scaffold database-first). Khai báo `DbSet<AppUser>`, `DbSet<Document>`, `DbSet<Subject>`, đọc chuỗi kết nối từ `appsettings.json`. **File dùng chung — xem mục 5.** |

Lưu ý: cả entity và `DbContext` đều đặt trong namespace `Prn212.AIStudyHub.DataAccess`, nên các nơi khác chỉ cần **một** dòng `using Prn212.AIStudyHub.DataAccess;` là dùng được cả entity lẫn `DbContext`.

### 4.2. `Prn212.AIStudyHub.Services` — Tầng nghiệp vụ

Chứa **logic** của ứng dụng: kiểm tra dữ liệu hợp lệ, mã hóa mật khẩu, copy file upload, truy vấn danh sách... Mỗi phương thức tự mở một `AistudyHubDbContext` rồi đóng lại (`using var context = new AistudyHubDbContext();`) — cách này tránh dữ liệu cũ và tự giải phóng kết nối.

| File | Ý nghĩa |
|---|---|
| `Auth/AuthService.cs` | Đăng nhập, đăng ký, và `GetDefaultUser()` (lấy tài khoản mẫu để giả lập session khi chưa có màn hình login). |
| `Auth/AccountService.cs` | Cập nhật profile, quên/đặt lại mật khẩu qua email. |
| `Documents/DocumentService.Commands.cs` | Phần **ghi** của `DocumentService`: `UploadAsync`, `UpdateMetadataAsync`, `DeleteAsync`. |
| `Documents/DocumentService.Queries.cs` | Phần **đọc** của `DocumentService`: `GetPaged` (list + tìm + lọc + sắp xếp + phân trang), `GetDetail`, `GetAllSubjects`. |

**Điểm cần hiểu — `partial class`:** `DocumentService` là **một class duy nhất nhưng được chia làm hai file** nhờ từ khóa `partial`. Nhờ đó giao diện chỉ cần một `new DocumentService()` để gọi mọi thứ, nhưng hai người Doc mỗi người sửa một file riêng → không đụng nhau. Cả hai file đều mở đầu bằng `public partial class DocumentService`.

### 4.3. `Prn212.AIStudyHub.WPF` — Tầng giao diện

Là project được chạy (startup). Chỉ lo hiển thị và gọi Service; **không** chứa logic nghiệp vụ hay truy cập database.

| File / Folder | Ý nghĩa |
|---|---|
| `Views/Auth/` | Các màn hình của Auth (đăng nhập, đăng ký, quên mật khẩu, profile). |
| `Views/Documents/` | Các màn hình tài liệu. `UploadDocumentWindow` nằm ở đây. |
| `Resources/` | Style dùng chung (`Styles.xaml`, converters...). |
| `App.xaml` | Khai báo `StartupUri="MainWindow.xaml"` — cửa sổ mở đầu tiên. |
| `App.xaml.cs` | Khi khởi động, đặt `CurrentUser` bằng tài khoản mẫu (session giả lập). |
| `MainWindow.xaml(.cs)` | Màn hình chính, chứa các nút mở màn hình con. **Vùng chung — xem mục 5.** |
| `appsettings.json` | Chuỗi kết nối database (mỗi máy tự tạo). |

Cách giao diện gọi service (mẫu trong `UploadDocumentWindow.xaml.cs`):

```csharp
private readonly DocumentService _documentService = new();
// ...
await _documentService.UploadAsync(userId, subjectId, title, filePath);
```

---

## 5. Quy ước chống conflict — phần QUAN TRỌNG NHẤT

### 5.1. Bốn quy tắc bắt buộc

1. **Ai làm tính năng nào, ở nguyên file/folder đó.** Nhóm Auth chỉ sửa `Auth/`; nhóm Document chỉ sửa `Documents/`.
2. **Giao diện KHÔNG được `new AistudyHubDbContext()`.** Luôn đi qua Service.
3. **Không tự ý sửa file dùng chung** (xem 5.2). Cần sửa thì báo nhóm trước.
4. **Mỗi tính năng làm trên một branch Git riêng.** (xem 5.3)

### 5.2. Các file dùng chung ("điểm nóng") và cách xử lý

| File | Cách tránh conflict |
|---|---|
| `AistudyHubDbContext.cs` | Scaffold từ database-first nên hạn chế sửa. Giao cho **1 người** (phụ trách hạ tầng) quản. Ai cần thêm bảng/entity thì báo người đó. |
| `MainWindow.xaml` (và `.cs`) | Đây là màn hình điều hướng chung, ai thêm màn hình mới cũng cần thêm 1 nút vào đây → dễ đụng. Quy ước: mỗi người thêm nút của mình vào **một khu vực riêng** trong `MainWindow`, và **báo nhóm** khi sửa file này. |
| `App.xaml` / `App.xaml.cs` | Chỉ chứa `StartupUri` và session — gần như không cần sửa. Đừng đụng vào trừ khi thật cần. |
| `Resources/Styles.xaml` | Nếu dùng style chung, tách thành nhiều file (mỗi nhóm một file) rồi merge, thay vì cùng sửa một file. |
| `appsettings.json` | Mỗi máy có chuỗi kết nối khác nhau. Đưa vào `.gitignore` và tạo `appsettings.example.json` làm mẫu, tránh ghi đè cấu hình của nhau. |

### 5.3. Quy ước Git

- Mỗi tính năng làm trên **branch riêng**: `feature/auth-login`, `feature/auth-reset-password`, `feature/doc-list`, `feature/doc-delete`...
- Commit **nhỏ và thường xuyên**, message rõ ràng.
- **`pull` nhánh chính mỗi ngày** để không dồn conflict về cuối.
- Mở **Pull Request** và nhờ 1 người khác review trước khi merge.
- Tuyệt đối không commit các thư mục `bin/`, `obj/`, `.vs/` (đã có trong `.gitignore`).

---

## 6. Hướng dẫn theo chức năng

### 6.1. Công thức chung khi làm MỘT tính năng (áp dụng cho mọi người)

1. **(Nếu cần)** thêm/sửa entity trong `DataAccess/Entities/` — nhưng entity dùng chung, sửa phải **báo nhóm**.
2. **Service** — thêm phương thức vào file service của mình (service tự mở `AistudyHubDbContext`, đặt toàn bộ logic ở đây).
3. **View** — dựng giao diện trong `Views/<nhóm>/`; trong code-behind tạo service bằng `new XxxService()` và gọi phương thức. **Không** `new AistudyHubDbContext()`.
4. **Điều hướng** — nếu màn hình cần mở từ màn hình chính, thêm nút vào `MainWindow` (báo nhóm vì là file chung), hoặc mở từ màn hình khác của nhóm mình.
5. **Chạy thử → commit → push branch → tạo Pull Request.**

### 6.2. Nhóm AUTH (2 người)

**Chức năng phụ trách:** đăng ký, đăng nhập, quên/đặt lại mật khẩu qua email, cập nhật profile, đăng xuất & session.

**File của nhóm:** `Services/Auth/*`, `WPF/Views/Auth/*`.

**Chia việc để 2 người KHÔNG đụng file nhau:**

- **AUTH 1 → `AuthService.cs`:** đăng ký, đăng nhập, session.
  - `Login(email, password)`: tìm user theo email → so khớp mật khẩu đã hash → trả về user hoặc null.
  - `Register(...)`: kiểm tra email chưa tồn tại → **mã hóa mật khẩu** (BCrypt/PBKDF2, không lưu mật khẩu thô) → thêm `AppUser` → lưu.
  - Session: quản lý `App.CurrentUser` (đăng nhập gán, đăng xuất gán null). (`GetDefaultUser()` hiện có là bản giả lập tạm.)
- **AUTH 2 → `AccountService.cs`:** profile + reset mật khẩu.
  - `UpdateProfile(userId, firstName, lastName)`.
  - `RequestPasswordReset(email)`: sinh token, gửi email (SMTP).
  - `ResetPassword(email, token, newPassword)`.

Hai người viết **hai class ở hai file khác nhau** nên gần như không đụng nhau. Việc cần thống nhất chung: nếu cần thêm truy vấn trên bảng `AppUser`, báo nhau để tránh viết trùng.

### 6.3. Nhóm DOCUMENT (2 người)

**Chức năng phụ trách:** upload (đã xong), xem danh sách + phân trang, chi tiết + preview, download, sửa metadata, xóa, tìm kiếm, lọc theo môn, sắp xếp.

**File của nhóm:** `Services/Documents/*`, `WPF/Views/Documents/*`.

**Chia việc theo "ghi" và "đọc" (mỗi người một file của cùng một class `partial`):**

- **DOC 1 → `DocumentService.Commands.cs`:** upload, sửa metadata, xóa, download.
  - `UploadAsync(userId, subjectId, title, sourceFilePath)` — **đã có sẵn làm mẫu**: copy file, sinh tên duy nhất, xác định `ContentType`, lưu bản ghi.
  - `UpdateMetadataAsync(...)`, `DeleteAsync(...)` — điền tiếp theo mẫu.
- **DOC 2 → `DocumentService.Queries.cs`:** danh sách + phân trang, chi tiết, danh sách môn.
  - `GetPaged(page, pageSize, keyword, subjectId, sortBy)` — gộp tìm/lọc/sắp xếp/phân trang, **đã có sẵn làm mẫu**.
  - `GetDetail(id)` cho màn hình chi tiết/preview.
  - `GetAllSubjects()` cho combobox lọc môn.

Vì là `partial class`, cả hai file cùng thuộc class `DocumentService`. Giao diện chỉ cần `new DocumentService()` là gọi được cả phần ghi lẫn phần đọc. Điểm cần thống nhất chung: **kiểu dữ liệu trả về danh sách** (hiện trả thẳng `Document`) — chốt sớm để màn hình danh sách và chi tiết ăn khớp.

---

## 7. Checklist trước khi tạo Pull Request

- [ ] Code của tôi nằm đúng file/folder chức năng của tôi.
- [ ] Không có chỗ nào `new AistudyHubDbContext()` trong View.
- [ ] Không sửa file dùng chung (DbContext, MainWindow, App.xaml) mà chưa báo nhóm.
- [ ] Đã `pull` nhánh chính và giải quyết hết conflict (nếu có).
- [ ] Project build và chạy được.
- [ ] Không commit `bin/`, `obj/`, `.vs/`, hay `appsettings.json` chứa thông tin máy cá nhân.

---

## 8. Ghi chú thêm

- **Vì sao mỗi phương thức tự mở DbContext?** Dùng `using var context = new AistudyHubDbContext();` trong từng phương thức giúp mỗi thao tác có kết nối riêng, tự đóng khi xong, tránh lỗi dữ liệu cũ giữa các lần gọi. Đơn giản và an toàn cho ứng dụng desktop.
- **DbContext vẫn chạy dù nằm ở project khác:** `appsettings.json` nằm ở project WPF và được copy ra thư mục chạy, nên `new AistudyHubDbContext()` vẫn đọc được chuỗi kết nối.
- **Nếu rubric môn học yêu cầu Repository pattern hoặc interface cho DI:** bản gọn này đã lược bỏ chúng cho đơn giản. Khi cần, có thể bổ sung lại tầng Repository và interface mà không phá vỡ cách chia folder hiện tại.
- **MVVM hay code-behind?** Bản này đang dùng code-behind (như phần Upload). Yêu cầu tối thiểu để đúng 3-layer là: View gọi **Service**, không gọi thẳng database. Nhóm thống nhất một hướng và làm nhất quán.
