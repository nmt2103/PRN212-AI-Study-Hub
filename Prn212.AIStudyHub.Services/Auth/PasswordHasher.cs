using System.Security.Cryptography;

namespace Prn212.AIStudyHub.Services.Auth;

/// <summary>
/// Băm và kiểm tra mật khẩu bằng PBKDF2 (chỉ dùng thư viện chuẩn .NET, không cần NuGet).
/// Định dạng lưu: "{iterations}.{salt-base64}.{hash-base64}".
/// </summary>
public static class PasswordHasher
{
  private const int Iterations = 100_000;
  private const int SaltSize = 16;
  private const int HashSize = 32;

  public static string Hash(string password)
  {
    byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
    byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
        password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
    return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
  }

  public static bool Verify(string password, string stored)
  {
    // Tài khoản mẫu (seeding.sql) lưu mật khẩu dạng thô, không có dấu chấm phân tách.
    // -> so sánh trực tiếp để vẫn đăng nhập được. Tài khoản đăng ký mới luôn là hash.
    var parts = stored.Split('.');
    if (parts.Length != 3)
      return stored == password;

    if (!int.TryParse(parts[0], out int iterations))
      return false;
    byte[] salt = Convert.FromBase64String(parts[1]);
    byte[] expected = Convert.FromBase64String(parts[2]);
    byte[] actual = Rfc2898DeriveBytes.Pbkdf2(
        password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
    return CryptographicOperations.FixedTimeEquals(actual, expected);
  }
}
