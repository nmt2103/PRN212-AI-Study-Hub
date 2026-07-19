using Prn212.AIStudyHub.DataAccess;

namespace Prn212.AIStudyHub.Services.Auth;

/// <summary>
/// Các chức năng dành riêng cho Admin (ví dụ: Quản lý người dùng).
/// </summary>
public class AdminService
{
  /// <summary>
  /// Lấy danh sách tất cả các người dùng có Role = "Student".
  /// </summary>
  public List<AppUser> GetStudents()
  {
    using var context = new AistudyHubDbContext();
    return context.AppUsers
      .Where(u => u.Role == "Student")
      .OrderByDescending(u => u.CreatedAt)
      .ToList();
  }

  /// <summary>
  /// Đảo ngược trạng thái IsActive của người dùng. Trả về trạng thái mới.
  /// </summary>
  public bool ToggleUserStatus(int userId)
  {
    using var context = new AistudyHubDbContext();

    var user = context.AppUsers.FirstOrDefault(u => u.Id == userId);
    if (user == null)
      throw new InvalidOperationException("Người dùng không tồn tại.");

    // Rule: Chỉ cho phép khóa/mở khóa tài khoản Student
    if (user.Role != "Student")
      throw new InvalidOperationException("Chỉ được phép thao tác trên tài khoản Sinh viên.");

    user.IsActive = !user.IsActive;
    context.SaveChanges();

    return user.IsActive;
  }
}
