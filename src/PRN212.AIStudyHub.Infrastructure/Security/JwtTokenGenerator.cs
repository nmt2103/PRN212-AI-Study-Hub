using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PRN212.AIStudyHub.Application.Interfaces.Security;
using PRN212.AIStudyHub.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PRN212.AIStudyHub.Infrastructure.Security;

public class JwtTokenGenerator(IOptions<JwtSettings> jwtOptions) : IJwtTokenGenerator
{
  private readonly JwtSettings _jwtSettings = jwtOptions.Value;

  public string GenerateToken(AppUser user)
  {
    var secretKey = _jwtSettings.Secret;
    if (string.IsNullOrEmpty(secretKey))
    {
      throw new InvalidOperationException("JWT secret key is not configured.");
    }

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = System.Text.Encoding.UTF8.GetBytes(secretKey);

    var claims = new List<Claim>
    {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new(ClaimTypes.Email, user.Email),
      new(ClaimTypes.GivenName, $"{user.LastName} {user.FirstName}"),
      new(ClaimTypes.Role, user.Role)
    };

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Issuer = _jwtSettings.Issuer,
      Audience = _jwtSettings.Audience,
      Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryMinutes / 60.0),
      SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key),
        SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
  }
}