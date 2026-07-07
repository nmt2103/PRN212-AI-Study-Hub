using Microsoft.EntityFrameworkCore;
using Prn212.AIStudyHub.DataAccess;

namespace Prn212.AIStudyHub.Services.Auth;

/// <summary>
/// Cập nhật profile / quên - đặt lại mật khẩu. NGƯỜI AUTH 2 làm ở file này.
/// </summary>
public class AccountService
{
  private readonly AistudyHubDbContext _context = new();

  public async Task UpdateProfile(string email, string firstName, string lastName)
  {
    AppUser? user = await _context.AppUsers.FirstOrDefaultAsync(e => e.Email.Equals(email));
    if (user == null)
      throw new Exception("User doesn't exist in the system.");
    user.FirstName = firstName;
    user.LastName = lastName;
    await _context.SaveChangesAsync();
  }

  public async Task<AppUser> GetCurrentUser(string email)
  {
    var user = await _context.AppUsers.FirstOrDefaultAsync(e => e.Email.Equals(email));
    if (user == null)
      throw new Exception("User doesn't exist in the system.");
    return user;
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
