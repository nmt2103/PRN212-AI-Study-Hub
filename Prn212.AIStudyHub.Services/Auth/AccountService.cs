using Prn212.AIStudyHub.DataAccess;

namespace Prn212.AIStudyHub.Services.Auth;

/// <summary>
/// Cập nhật profile / quên - đặt lại mật khẩu. NGƯỜI AUTH 2 làm ở file này.
/// </summary>
public class AccountService
{
  public void UpdateProfile(int userId, string firstName, string lastName)
  {
    // TODO (AUTH 2): lấy user theo id, cập nhật tên, SaveChanges.
    throw new NotImplementedException("Nhóm AUTH cài đặt UpdateProfile tại đây.");
  }

  public void RequestPasswordReset(string email)
  {
    // TODO (AUTH 2): sinh token, gửi email.
    throw new NotImplementedException("Nhóm AUTH cài đặt RequestPasswordReset tại đây.");
  }

  public void ResetPassword(string email, string token, string newPassword)
  {
    // TODO (AUTH 2): kiểm tra token, hash mật khẩu mới, cập nhật.
    throw new NotImplementedException("Nhóm AUTH cài đặt ResetPassword tại đây.");
  }
}
