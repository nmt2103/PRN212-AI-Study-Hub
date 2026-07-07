using Prn212.AIStudyHub.DataAccess;

namespace Prn212.AIStudyHub.Services.Auth;

/// <summary>
/// Đăng nhập / đăng ký. FILE CỦA AUTH 1.
/// (Đăng xuất chỉ là xóa session ở tầng giao diện: App.CurrentUser = null)
/// </summary>
public class AuthService
{
  /// <summary>
  /// Đăng nhập: trả về AppUser nếu email + mật khẩu đúng, ngược lại trả về null.
  /// </summary>
  public AppUser? Login(string email, string password)
  {
    using var context = new AistudyHubDbContext();

    var user = context.AppUsers.FirstOrDefault(u => u.Email == email);
    if (user == null)
      return null;          // không có email này
    if (!user.IsActive)
      return null;        // tài khoản bị khóa
    if (!PasswordHasher.Verify(password, user.PasswordHash))
      return null; // sai mật khẩu

    return user;
  }

  /// <summary>
  /// Đăng ký tài khoản mới. Ném lỗi nếu email đã tồn tại.
  /// </summary>
  public AppUser Register(string email, string password, string firstName, string lastName)
  {
    // TODO (AUTH 1): tìm user theo email, so khớp mật khẩu đã HASH.
    using var context = new AistudyHubDbContext();

    if (context.AppUsers.Any(u => u.Email == email))
      throw new InvalidOperationException("Email này đã được đăng ký.");

    var user = new AppUser
    {
      Email = email.Trim(),
      PasswordHash = PasswordHasher.Hash(password), // luôn HASH, không lưu mật khẩu thô
      FirstName = firstName.Trim(),
      LastName = lastName.Trim(),
      Role = "Student",
      IsActive = true,
      CreatedAt = DateTime.UtcNow
    };

    context.AppUsers.Add(user);
    context.SaveChanges();
    return user;
  }
}
