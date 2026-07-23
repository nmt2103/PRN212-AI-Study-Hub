using Microsoft.EntityFrameworkCore;
using Prn212.AIStudyHub.DataAccess;

namespace Prn212.AIStudyHub.Services.Auth;

/// <summary>
/// Cập nhật profile / quên - đặt lại mật khẩu. NGƯỜI AUTH 2 làm ở file này.
/// </summary>
public class AccountService
{
    public async Task UpdateProfile(string email, string firstName, string lastName)
    {
        using var context = new AistudyHubDbContext();
        AppUser? user = await context.AppUsers.FirstOrDefaultAsync(e => e.Email.Equals(email));
        if (user == null)
            throw new InvalidOperationException("Tài khoản không tồn tại trên hệ thống.");
        user.FirstName = firstName;
        user.LastName = lastName;
        await context.SaveChangesAsync();
    }

    public async Task<AppUser> GetCurrentUser(string email)
    {
        using var context = new AistudyHubDbContext();
        var user = await context.AppUsers.FirstOrDefaultAsync(e => e.Email.Equals(email));
        if (user == null)
            throw new InvalidOperationException("Tài khoản không tồn tại trên hệ thống.");
        return user;
    }

    public async Task RequestPasswordReset(string email, string newPassword, string currentPassword)
    {
        using var context = new AistudyHubDbContext();
        var user = await context.AppUsers.FirstOrDefaultAsync(e => e.Email.Equals(email));
        if (user == null)
        {
            throw new InvalidOperationException("Tài khoản không tồn tại trên hệ thống.");
        }
        if(newPassword.Equals(currentPassword))
        {
            throw new InvalidOperationException("Mật khẩu mới không được trùng mật khẩu cũ");
        }
        bool isMatch = PasswordHasher.Verify(currentPassword, user.PasswordHash);
        if (!isMatch)
        {
            throw new InvalidOperationException("Mật khẩu không khớp dưới hệ thống");
        }
        user.PasswordHash = PasswordHasher.Hash(newPassword);
        await context.SaveChangesAsync();

    }

    public void ResetPassword(string email, string token, string newPassword)
    {
        // TODO (AUTH 2): kiểm tra token, hash mật khẩu mới, cập nhật.
        throw new NotImplementedException("Nhóm AUTH cài đặt ResetPassword tại đây.");
    }
}
