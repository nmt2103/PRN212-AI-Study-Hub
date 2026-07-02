# AIStudyHub — Tài liệu bàn giao kiến trúc 3-Layer

> Tài liệu dành cho **4 thành viên** trong nhóm. Đọc kỹ trước khi bắt đầu code.
> Mục tiêu: mỗi người làm ở folder riêng, hạn chế tối đa conflict khi push lên GitHub.

---

## 1. Tài liệu này để làm gì

Trước đây toàn bộ code nằm trong **một project WPF duy nhất**, entity và `DbContext` để lẫn nhau, và phần Upload gọi thẳng database ngay trong code-behind của giao diện. Cách này khiến 4 người dễ sửa trùng file → conflict liên tục.

Tài liệu này mô tả cấu trúc **3-layer** mới: giải thích *từng project, từng folder, từng file làm gì và tại sao*, kèm **hướng dẫn cụ thể cho từng nhóm chức năng** (Auth và Document) cần làm gì, ở đâu, theo thứ tự nào.

---

## 2. Kiến trúc 3-Layer là gì và tại sao chọn nó

Ứng dụng được chia thành 3 tầng, cộng thêm 1 project chứa các lớp dữ liệu dùng chung:

| Tầng | Project | Trách nhiệm |
|---|---|---|
| Presentation (giao diện) | `Prn212.AIStudyHub.WPF` | Hiển thị, nhận thao tác người dùng. **Không** đụng database. |
| Business (nghiệp vụ) | `Prn212.AIStudyHub.Business` | Logic: kiểm tra dữ liệu, mã hóa mật khẩu, xử lý file upload... |
| Data Access (dữ liệu) | `Prn212.AIStudyHub.DataAccess` | Đọc/ghi database qua Entity Framework. |
| Dùng chung | `Prn212.AIStudyHub.Entities` | Các lớp dữ liệu `AppUser`, `Document`, `Subject`. |

**Quy tắc vàng — phải nhớ:** tầng trên được gọi tầng dưới, **không bao giờ ngược lại**.

```
View (WPF)  →  Service (Business)  →  Repository (DataAccess)  →  AistudyHubDbContext  →  SQL Server
                     ↑ tất cả các tầng cùng dùng chung project Entities ↑
```

Ví dụ đúng: khi bấm nút "Upload", `UploadDocumentWindow` gọi `IDocumentService.UploadAsync(...)`, service này gọi `IDocumentRepository.Add(...)`, repository mới thao tác `AistudyHubDbContext`.

Ví dụ **sai** (tuyệt đối tránh): giao diện tự viết `new AistudyHubDbContext()` rồi `.SaveChanges()`. (Đây chính là chỗ code Upload hiện tại đang mắc và sẽ được sửa lại.)

**Lợi ích cho nhóm:**
- **Chống conflict:** mỗi người làm trọn một tính năng theo *chiều dọc* (đi qua cả 3 tầng), nên phần lớn file chỉ 1 người đụng.
- **Không phải chờ nhau:** cả nhóm thống nhất *interface* trước, sau đó mỗi người tự code phần của mình dựa trên interface đó.
- **Dễ test, dễ sửa:** logic tách khỏi giao diện, lỗi ở đâu tìm đúng tầng đó.

---

## 3. Sơ đồ tổng thể

```
Prn212.AIStudyHub.slnx
│
├── Prn212.AIStudyHub.Entities/          # Lớp dữ liệu dùng chung
│   ├── AppUser.cs
│   ├── Document.cs
│   └── Subject.cs
│
├── Prn212.AIStudyHub.DataAccess/        # Tầng dữ liệu
│   ├── AistudyHubDbContext.cs           # File chung — chỉ 1 người quản
│   └── Repositories/
│       ├── Auth/                        # ⟵ Nhóm AUTH
│       │   ├── IUserRepository.cs
│       │   └── UserRepository.cs
│       └── Documents/                   # ⟵ Nhóm DOCUMENT
│           ├── IDocumentRepository.cs        # ghi:  thêm/sửa/xóa
│           ├── DocumentRepository.cs
│           ├── IDocumentQueryRepository.cs   # đọc:  list/tìm/lọc/sắp xếp
│           ├── DocumentQueryRepository.cs
│           ├── ISubjectRepository.cs
│           └── SubjectRepository.cs
│
├── Prn212.AIStudyHub.Business/          # Tầng nghiệp vụ
│   ├── Services/
│   │   ├── Auth/                        # ⟵ Nhóm AUTH
│   │   │   ├── IAuthService.cs
│   │   │   ├── AuthService.cs
│   │   │   ├── IAccountService.cs
│   │   │   └── AccountService.cs
│   │   └── Documents/                   # ⟵ Nhóm DOCUMENT
│   │       ├── IDocumentService.cs           # upload/download/sửa/xóa
│   │       ├── DocumentService.cs
│   │       ├── IDocumentQueryService.cs      # list/tìm/lọc/sắp xếp
│   │       └── DocumentQueryService.cs
│   └── DependencyInjection/
│       ├── AuthModule.cs                # ⟵ AUTH tự đăng ký DI
│       └── DocumentModule.cs            # ⟵ DOCUMENT tự đăng ký DI
│
└── Prn212.AIStudyHub.WPF/               # Tầng giao diện (project chạy)
    ├── Views/
    │   ├── Auth/                        # ⟵ Nhóm AUTH
    │   ├── Documents/                   # ⟵ Nhóm DOCUMENT (UploadDocumentWindow ở đây)
    │   └── Shared/                      # MainWindow — vùng chung
    ├── ViewModels/                      # (chỉ dùng nếu nhóm theo MVVM)
    │   ├── Auth/
    │   ├── Documents/
    │   └── Shared/
    ├── Resources/
    │   ├── Styles.xaml                  # dictionary gốc — chỉ merge, không viết style riêng
    │   ├── Styles.Auth.xaml             # ⟵ AUTH
    │   └── Styles.Documents.xaml        # ⟵ DOCUMENT
    ├── App.xaml / App.xaml.cs           # chỉ gọi các Module — ít khi sửa
    ├── appsettings.json                 # chuỗi kết nối database
    └── MainWindow.xaml
```

Nguyên tắc chia folder: **tách theo TÍNH NĂNG (Auth / Document), không tách theo TẦNG.** Nhờ vậy nhóm Auth chỉ đụng các folder `Auth/`, nhóm Document chỉ đụng các folder `Documents/` — gần như không giẫm chân nhau.

---

## 4. Giải thích chi tiết từng project

### 4.1. `Prn212.AIStudyHub.Entities` — Lớp dữ liệu dùng chung

Chứa các "lớp dữ liệu thuần" (POCO) ánh xạ với các bảng trong database. Không chứa logic, không phụ thuộc thư viện nào.

| File | Ý nghĩa |
|---|---|
| `AppUser.cs` | Một tài khoản người dùng: `Email`, `PasswordHash`, `FirstName`, `LastName`, `Role`, `IsActive`, `CreatedAt`... |
| `Document.cs` | Một tài liệu đã upload: `Title`, `FileName`, `StoragePath`, `FileSize`, `FileExtension`, `ContentType`, `UploadedAt`, khóa ngoại `UserId`, `SubjectId`. |
| `Subject.cs` | Một môn học: `Name`, `Description`. |

**Tại sao tách riêng:** cả 3 tầng đều cần dùng các lớp này. Để chung một chỗ thì tầng nào cũng tham chiếu được mà không sinh phụ thuộc vòng.

### 4.2. `Prn212.AIStudyHub.DataAccess` — Tầng dữ liệu

Nơi **duy nhất** được phép nói chuyện trực tiếp với database. Gói Entity Framework Core nằm ở đây.

| File / Folder | Ý nghĩa |
|---|---|
| `AistudyHubDbContext.cs` | Đại diện cho kết nối database (scaffold từ database-first). Khai báo `DbSet<AppUser>`, `DbSet<Document>`, `DbSet<Subject>` và cấu hình bảng. **Đây là file dùng chung — xem mục 5.** |
| `Repositories/Auth/IUserRepository.cs` | *Interface* mô tả các thao tác DB liên quan user (tìm theo email, thêm user, cập nhật mật khẩu...). |
| `Repositories/Auth/UserRepository.cs` | Phần *cài đặt thật* của `IUserRepository`, dùng `AistudyHubDbContext`. |
| `Repositories/Documents/IDocumentRepository.cs` | Interface **ghi** dữ liệu tài liệu: `Add`, `Update`, `Delete`. |
| `Repositories/Documents/DocumentRepository.cs` | Cài đặt phần ghi. |
| `Repositories/Documents/IDocumentQueryRepository.cs` | Interface **đọc** dữ liệu tài liệu: lấy danh sách có phân trang, tìm kiếm, lọc theo môn, sắp xếp, lấy chi tiết. |
| `Repositories/Documents/DocumentQueryRepository.cs` | Cài đặt phần đọc. |
| `Repositories/Documents/ISubjectRepository.cs` + `.cs` | Lấy danh sách môn học (đổ vào combobox). |

**Tại sao có interface cho repository:** để tầng Business chỉ phụ thuộc vào *hợp đồng* (interface), không phụ thuộc chi tiết. Nhờ đó dễ thay thế, dễ viết unit test.

**Tại sao tách Document thành "ghi" và "đọc":** 2 thành viên nhóm Document cùng làm việc trên bảng `Document`. Nếu để chung 1 file `DocumentRepository.cs`, hai người sẽ liên tục sửa trùng file. Tách đôi → mỗi người một file, không conflict.

### 4.3. `Prn212.AIStudyHub.Business` — Tầng nghiệp vụ

Chứa **logic** của ứng dụng. Đây là nơi kiểm tra dữ liệu hợp lệ, mã hóa mật khẩu, copy file upload, sinh token reset mật khẩu... Tầng này gọi xuống Repository, và được tầng giao diện gọi lên.

| File / Folder | Ý nghĩa |
|---|---|
| `Services/Auth/IAuthService.cs` + `.cs` | Đăng ký, đăng nhập, quản lý phiên đăng nhập (session). |
| `Services/Auth/IAccountService.cs` + `.cs` | Cập nhật profile, quên/đặt lại mật khẩu qua email. (Tách khỏi `AuthService` để 2 người Auth không sửa chung 1 file.) |
| `Services/Documents/IDocumentService.cs` + `.cs` | Upload, download, sửa metadata, xóa tài liệu. Chứa logic copy file, sinh tên file duy nhất, xác định `ContentType`. |
| `Services/Documents/IDocumentQueryService.cs` + `.cs` | Lấy danh sách (phân trang), tìm kiếm, lọc theo môn, sắp xếp, xem chi tiết/preview. |
| `DependencyInjection/AuthModule.cs` | Hàm mở rộng `AddAuthModule(...)` đăng ký toàn bộ repository + service của Auth. |
| `DependencyInjection/DocumentModule.cs` | Hàm mở rộng `AddDocumentModule(...)` đăng ký toàn bộ repository + service của Document. |

**Tại sao có các "Module" DI riêng:** thay vì tất cả cùng sửa `App.xaml.cs` để đăng ký service (chắc chắn conflict), mỗi nhóm chỉ sửa file Module của mình. `App.xaml.cs` chỉ gọi `.AddAuthModule().AddDocumentModule()` một lần và gần như không đổi nữa.

### 4.4. `Prn212.AIStudyHub.WPF` — Tầng giao diện

Là project được chạy (startup). Chỉ lo hiển thị và gọi Service; **không được** chứa logic nghiệp vụ hay truy cập database.

| Folder | Ý nghĩa |
|---|---|
| `Views/Auth/` | Các cửa sổ/màn hình của Auth (đăng nhập, đăng ký, quên mật khẩu, profile). |
| `Views/Documents/` | Các màn hình tài liệu. `UploadDocumentWindow` được chuyển vào đây. |
| `Views/Shared/` | `MainWindow` và các thành phần chung (thanh điều hướng, dialog dùng lại). |
| `ViewModels/*` | (Nếu nhóm theo MVVM) lớp trung gian giữa View và Service, giữ trạng thái màn hình. |
| `Resources/Styles.xaml` | Dictionary gốc, chỉ dùng để *merge* các file style con. |
| `Resources/Styles.Auth.xaml` / `Styles.Documents.xaml` | Style riêng của từng nhóm — mỗi nhóm sửa file của mình. |
| `App.xaml.cs` | Khởi tạo DI container, gọi các Module, chứa `CurrentUser` (session). |
| `appsettings.json` | Chuỗi kết nối database. |

---

## 5. Quy ước chống conflict — phần QUAN TRỌNG NHẤT

### 5.1. Bốn quy tắc bắt buộc

1. **Chốt interface trước, code sau.** Khi bắt đầu một tính năng, viết interface (VD `IDocumentQueryService`) rồi commit ngay. Người khác cần dùng thì code dựa trên interface đó, không phải chờ bạn viết xong phần cài đặt.
2. **Ai làm tính năng nào, ở nguyên folder đó.** Nhóm Auth chỉ sửa các folder `Auth/`; nhóm Document chỉ sửa `Documents/`.
3. **Không tự ý sửa file dùng chung** (xem 5.2). Cần sửa thì báo nhóm trước.
4. **Giao diện KHÔNG được `new AistudyHubDbContext()`.** Luôn đi qua Service.

### 5.2. Các file dùng chung ("điểm nóng") và cách xử lý

| File | Cách tránh conflict |
|---|---|
| `AistudyHubDbContext.cs` | File này scaffold từ database-first nên hạn chế sửa. Giao cho **1 người** (phụ trách hạ tầng) quản. Ai cần thêm bảng/entity thì báo người đó cập nhật. |
| `App.xaml.cs` | Chỉ gọi các Module. Mỗi nhóm đăng ký service trong Module riêng của mình, **không** sửa trực tiếp `App.xaml.cs`. |
| `Styles.xaml` (gốc) | Chỉ chứa các dòng `MergedDictionaries`. Style riêng viết trong `Styles.Auth.xaml` / `Styles.Documents.xaml`. |
| `appsettings.json` | Mỗi máy có chuỗi kết nối khác nhau. Nên đưa vào `.gitignore` và tạo `appsettings.example.json` làm mẫu, tránh ghi đè cấu hình của nhau. |

### 5.3. Quy ước Git

- Mỗi tính năng làm trên **branch riêng**: `feature/auth-login`, `feature/auth-reset-password`, `feature/doc-search`, `feature/doc-delete`...
- Commit **nhỏ và thường xuyên**, message rõ ràng bằng tiếng Việt hoặc tiếng Anh nhất quán.
- **`pull` nhánh chính mỗi ngày** để không dồn conflict về cuối.
- Mở **Pull Request** và nhờ 1 người khác review trước khi merge.
- Tuyệt đối không commit các thư mục `bin/`, `obj/`, `.vs/` (đã có trong `.gitignore`).

---

## 6. Hướng dẫn theo chức năng

### 6.1. Công thức chung khi làm MỘT tính năng (áp dụng cho mọi người)

Luôn đi theo *chiều dọc*, từ dưới lên hoặc từ interface ra:

1. **(Nếu cần)** thêm/sửa entity trong `Entities/` — nhưng entity là dùng chung, nếu sửa phải **báo nhóm**.
2. **Repository** — viết interface + cài đặt trong `DataAccess/Repositories/<nhóm>/` (nếu cần thao tác DB mới).
3. **Service** — viết interface + cài đặt trong `Business/Services/<nhóm>/`, đặt toàn bộ logic ở đây.
4. **Đăng ký DI** — thêm dòng đăng ký repository/service vào Module của nhóm mình.
5. **ViewModel** (nếu MVVM) hoặc **code-behind** — gọi Service, **không** gọi thẳng DbContext.
6. **View + Style** — dựng giao diện trong `Views/<nhóm>/`, style trong `Styles.<nhóm>.xaml`.
7. **Chạy thử → commit → push branch → tạo Pull Request.**

### 6.2. Nhóm AUTH (2 người)

**Chức năng phụ trách:** đăng ký, đăng nhập, quên/đặt lại mật khẩu qua email, cập nhật profile, đăng xuất & quản lý session.

**File của nhóm:**
`DataAccess/Repositories/Auth/*`, `Business/Services/Auth/*`, `Business/DependencyInjection/AuthModule.cs`, `WPF/Views/Auth/*`, `WPF/ViewModels/Auth/*`, `WPF/Resources/Styles.Auth.xaml`.

**Gợi ý chia việc để 2 người KHÔNG đụng file nhau:**

- **Người 1 → `AuthService` / `IAuthService`:** đăng ký, đăng nhập, đăng xuất/session.
  - `RegisterAsync`: kiểm tra email chưa tồn tại → **mã hóa mật khẩu** (dùng BCrypt hoặc PBKDF2, tuyệt đối không lưu mật khẩu thô) → tạo `AppUser`.
  - `LoginAsync`: tìm user theo email → so khớp mật khẩu đã hash → trả về user hoặc báo sai.
  - Session: quản lý `App.CurrentUser` (đăng nhập gán, đăng xuất gán null).
- **Người 2 → `AccountService` / `IAccountService`:** profile + reset mật khẩu.
  - `UpdateProfileAsync`: cập nhật `FirstName`, `LastName`... rồi lưu.
  - `RequestPasswordResetAsync`: sinh token, gửi email (SMTP).
  - `ResetPasswordAsync`: kiểm tra token → cập nhật `PasswordHash`.

Vì hai người viết **hai service ở hai file khác nhau**, và mỗi service dùng chung `IUserRepository`, nên phần lớn thời gian không sửa trùng file. Chỉ cần **chốt `IUserRepository` trước** (cả hai cùng thống nhất các phương thức cần có), rồi ai cần thêm phương thức mới thì báo nhau.

### 6.3. Nhóm DOCUMENT (2 người)

**Chức năng phụ trách:** upload (đã xong), xem danh sách + phân trang, xem chi tiết + preview, download, sửa metadata, xóa, tìm kiếm, lọc theo môn, sắp xếp.

**File của nhóm:**
`DataAccess/Repositories/Documents/*`, `Business/Services/Documents/*`, `Business/DependencyInjection/DocumentModule.cs`, `WPF/Views/Documents/*`, `WPF/ViewModels/Documents/*`, `WPF/Resources/Styles.Documents.xaml`.

**Gợi ý chia việc theo "ghi" và "đọc":**

- **Người 3 → phía GHI (`DocumentService` + `DocumentRepository`):** upload, sửa metadata, xóa, download.
  - Việc đầu tiên: **chuyển logic Upload hiện có** từ `UploadDocumentWindow.xaml.cs` vào `DocumentService.UploadAsync(...)` — gồm copy file, sinh tên file duy nhất, xác định `ContentType`, lưu bản ghi. Sau đó View chỉ còn gọi `_documentService.UploadAsync(...)`.
  - `UpdateMetadataAsync`, `DeleteAsync`, `GetFileForDownloadAsync`.
- **Người 4 → phía ĐỌC (`DocumentQueryService` + `DocumentQueryRepository`):** danh sách + phân trang, chi tiết + preview, tìm kiếm, lọc theo môn, sắp xếp; kèm `SubjectRepository` để đổ combobox môn học.
  - `GetPagedAsync(page, pageSize, keyword, subjectId, sortBy)` — gộp tìm/lọc/sắp xếp/phân trang vào một phương thức truy vấn.
  - `GetDetailAsync(id)` cho màn hình chi tiết/preview.

Hai người dùng chung entity `Document` nhưng viết **hai cặp service/repository tách biệt** nên không sửa trùng file. Điểm cần thống nhất chung: **kiểu dữ liệu trả về danh sách** (ví dụ một lớp `DocumentListItem` hoặc trả thẳng `Document`) — chốt sớm để màn hình danh sách và màn hình chi tiết ăn khớp nhau.

---

## 7. Checklist trước khi tạo Pull Request

- [ ] Code của tôi nằm đúng folder chức năng của tôi.
- [ ] Không có chỗ nào `new AistudyHubDbContext()` trong View/ViewModel.
- [ ] Đã đăng ký service/repository mới vào Module của nhóm tôi.
- [ ] Không sửa file dùng chung (DbContext, App.xaml.cs, Styles.xaml gốc) mà chưa báo nhóm.
- [ ] Đã `pull` nhánh chính và giải quyết hết conflict (nếu có).
- [ ] Project build và chạy được.
- [ ] Không commit `bin/`, `obj/`, `.vs/`, hay `appsettings.json` chứa thông tin máy cá nhân.

---

## 8. Ghi chú thêm

- **MVVM hay code-behind?** Cấu trúc này chạy được với cả hai. Yêu cầu tối thiểu để đúng 3-layer là: View/ViewModel gọi **Service**, không gọi thẳng database. Nhóm thống nhất chọn một hướng và làm nhất quán.
- **DbContext vẫn chạy sau khi chuyển sang DataAccess:** vì `appsettings.json` vẫn nằm ở project WPF và được copy ra thư mục chạy, nên `new AistudyHubDbContext()` vẫn đọc được chuỗi kết nối như cũ. (Khuyến khích về sau tiêm `DbContext` qua DI để sạch hơn.)
- **Thứ tự khởi động dự án:** nhóm nên cùng ngồi lại chốt toàn bộ **interface** (`IUserRepository`, `IAuthService`, `IDocumentService`, `IDocumentQueryService`...) trong buổi đầu. Khi interface đã cố định, 4 người code song song mà không phải chờ nhau — đây là chìa khóa để làm nhóm không conflict.
