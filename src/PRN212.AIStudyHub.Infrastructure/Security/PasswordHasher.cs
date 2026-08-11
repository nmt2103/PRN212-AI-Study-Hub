using PRN212.AIStudyHub.Application.Interfaces.Security;

namespace PRN212.AIStudyHub.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
  public string HashPassword(string password)
  {
    return BCrypt.Net.BCrypt.HashPassword(password);
  }

  public bool VerifyPassword(string providedPassword, string hashedPassword)
  {
    return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
  }
}