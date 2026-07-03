using Prn212.AIStudyHub.DataAccess;

namespace Prn212.AIStudyHub.Services.Auth;

/// <summary>
/// Đăng ký / đăng nhập / session. NGƯỜI AUTH 1 làm ở file này.
/// </summary>
public class AuthService
{
  /// <summary>Lấy 1 tài khoản mẫu để giả lập đăng nhập (tạm dùng khi chưa có màn hình login).</summary>
  public AppUser? GetDefaultUser()
  {
    using var context = new AistudyHubDbContext();
    return context.AppUsers.FirstOrDefault(u => u.Email == "thuannm@aistudyhub.com")
           ?? context.AppUsers.FirstOrDefault();
  }

  public AppUser? Login(string email, string password)
  {
    // TODO (AUTH 1): tìm user theo email, so khớp mật khẩu đã HASH.
    using var context = new AistudyHubDbContext();
    // var user = context.AppUsers.FirstOrDefault(u => u.Email == email);
    // if (user == null || !VerifyPassword(password, user.PasswordHash)) return null;
    // return user;
    throw new NotImplementedException("Nhóm AUTH cài đặt Login tại đây.");
  }

  public AppUser Register(string email, string password, string firstName, string lastName)
  {
    // TODO (AUTH 1): kiểm tra email tồn tại chưa, HASH mật khẩu, thêm AppUser, SaveChanges.
    throw new NotImplementedException("Nhóm AUTH cài đặt Register tại đây.");
  }
}
