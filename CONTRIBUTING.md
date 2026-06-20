# Contributing - Quy trình làm việc nhóm

Hướng dẫn nhanh gọn về cách làm việc với dự án này.

---

## 📌 Quy trình cơ bản

### 1. Nhận Task

- Vào GitHub Issues, tìm task cần làm
- Click "Assignees" → chọn tên bạn
- Chuyển trạng thái issue sang "In Progress" trên Project board

### 2. Tạo Branch

```bash
git checkout main
git pull origin main
git checkout -b feature/XXX-feature-name
# NOTE: XXX là số thứ tự của issue trên GitHub, ví dụ: #1
```

**Naming convention:**

- `feature/XXX-feature-name`
- `fix/bug-number-description`

### 3. Code & Commit

```bash
# Commit thường xuyên
git commit -m "[TASK-XXX] <Short Description>"

git push origin feature/XXX-feature-name
```

### 4. Testing

```bash
# Run tests before submitting PR
dotnet test

# Build check
dotnet build
```

### 5. Submit PR

- Tạo PR trên GitHub: `main` ← `task/XXX`
- Link issue: `Closes #XXX`
- Request reviewer (1-2 người)

### 6. Code Review & Merge

- Address feedback từ reviewers
- Merge khi approved (≥1 approval)
- Delete branch sau khi merge

---

## 💡 Best Practices

### Git Commits

- ✅ **Commit thường xuyên**: Mỗi logical change = 1 commit
- ✅ **Meaningful messages**: "Fix login validation" chứ không phải "fix stuff"
- ✅ **Reference issue**: Luôn link đến GitHub issue `[TASK-XXX]`

**Good commit:**

```
[TASK-1.1] Create User model and DbContext
```

### Code Style

- **C# naming**: `PascalCase` cho classes/methods, `_camelCase` cho private fields
- **Indentation**: 4 spaces (không dùng tabs)
- **Line length**: Max 120 characters
- **No hardcoded values**: Dùng constants/config

**Good:**

```csharp
public class UserService
{
    private readonly IUserRepository _userRepository;

    public async Task<Result> RegisterAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            return new Result { IsSuccess = false };

        return await _userRepository.AddAsync(user);
    }
}
```

### Testing

| _NOTE: Không nghiêm ngặt_

- **Coverage >= 80%**: Viết tests cho logic mới
- **Test structure**: Arrange → Act → Assert
- **Run tests locally**: `dotnet test` trước PR

```csharp
[Fact]
public void RegisterUser_WithValidEmail_ShouldCreateAccount()
{
    // Arrange
    var service = new AuthService(_repository);

    // Act
    var result = service.RegisterAsync("user@test.com", "Pass123!").Result;

    // Assert
    Assert.True(result.IsSuccess);
}
```

### Database

- **Migrations**: Luôn tạo migration khi thay đổi model
  ```bash
  dotnet ef migrations add DescriptionOfChange
  dotnet ef database update
  ```
- **Meaningful names**: `CreateUserTable` chứ không phải `Update1`
- **Commit migrations**: Migrations là một phần của code

### Documentation

| _NOTE: Không nghiêm ngặt_

- **Code comments**: Chỉ comment logic phức tạp
- **XML docs**: Các public methods/classes cần có
  ```csharp
  /// <summary>
  /// Validates if email is correct format
  /// </summary>
  public bool IsValidEmail(string email)
  ```
- **Update docs**: Khi thay đổi flow, database, setup

---

## ✅ Checklist trước PR

| _NOTE: Nên thực hiện_

- [ ] Meaningful commit messages
- [ ] PR description có issue reference
- [ ] Reviewers assigned

| _NOTE: Không nghiêm ngặt_

- [ ] Branch từ `main` (up to date)
- [ ] Code written theo style guide
- [ ] Unit tests written (coverage >= 80%)
- [ ] All tests passing locally (`dotnet test`)
- [ ] No build warnings (`dotnet build`)

---

## 🚫 Quy tắc bắt buộc

- ❌ Không commit code có syntax errors
- ❌ Không commit secrets/credentials
- ✅ Tuân thủ quy trình: Branch → Code → Test → PR → Review → Merge

---

## 📞 Questions?

- Comment trong PR nếu có góp ý hoặc thắc mắc
- Có thể nhắn hỏi trong zalo

---

**Happy coding! 🚀**
