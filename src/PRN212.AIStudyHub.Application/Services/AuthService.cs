using Microsoft.EntityFrameworkCore;
using PRN212.AIStudyHub.Application.DTOs.Auth;
using PRN212.AIStudyHub.Application.Interfaces;
using PRN212.AIStudyHub.Application.Interfaces.Security;
using PRN212.AIStudyHub.Domain.Entities;
using System.Security.Cryptography;

namespace PRN212.AIStudyHub.Application.Services;

public class AuthService(IAppDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
  public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
  {
    // Validate the input parameters
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
    {
      throw new ArgumentException("Email and password are required");
    }

    // Query the user from the database based on the provided email
    var user = await context.AppUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

    // Check if the user exists and if the provided password matches the stored password hash
    if (user is null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
    {
      throw new InvalidOperationException("Invalid credentials");
    }

    // Check if the user account is active
    if (!user.IsActive)
    {
      throw new InvalidOperationException("User account is inactive");
    }

    return await GenerateAuthResponseAsync(user, cancellationToken);
  }

  public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
  {
    // Validate the input parameters
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.ConfirmPassword) || string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
    {
      throw new ArgumentException("Email, password, and confirm password are required");
    }

    // Check if the email already exists in the database
    var existingEmail = await context.AppUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

    if (existingEmail is not null)
    {
      throw new InvalidOperationException("Email already exists");
    }

    // Validate password and confirm password
    if (request.Password != request.ConfirmPassword)
    {
      throw new ArgumentException("Passwords do not match");
    }

    // Validate password length
    if (request.Password.Length < 6)
    {
      throw new ArgumentException("Password must be at least 6 characters long");
    }

    // Create a new AppUser entity and set its properties
    var newUser = new AppUser
    {
      Email = request.Email,
      PasswordHash = passwordHasher.HashPassword(request.Password),
      FirstName = request.FirstName,
      LastName = request.LastName,
      Role = request.Role ?? "Student",
      IsActive = true,
    };

    // Add the new user to the database and save changes
    context.AppUsers.Add(newUser);
    await context.SaveChangesAsync(cancellationToken);

    return await GenerateAuthResponseAsync(newUser, cancellationToken);
  }

  public async Task<UserDto> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    // Query the user from the database based on the provided Id
    var user = await context.AppUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    // Check if user found
    if (user is null)
      throw new KeyNotFoundException("User not found");

    // Return the user profile info
    return new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.Role, user.IsActive, user.CreatedAt, user.UpdatedAt);
  }

  private async Task<AuthResponse> GenerateAuthResponseAsync(AppUser user, CancellationToken cancellationToken)
  {
    // Generate access token and refresh token
    string accessToken = jwtTokenGenerator.GenerateToken(user);
    string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    // Create a new RefreshToken entity and set its properties
    var refreshTokenEntity = new RefreshToken
    {
      UserId = user.Id,
      Token = refreshToken,
      ExpiresAt = DateTime.UtcNow.AddDays(7),
      IsRevoked = false
    };

    // Add the refresh token entity to the database context and save changes
    context.RefreshTokens.Add(refreshTokenEntity);
    await context.SaveChangesAsync(cancellationToken);

    // Create a UserDto object to include user information in the response
    var userDto = new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.Role, user.IsActive, user.CreatedAt, user.UpdatedAt);

    // Return the authentication response with the generated tokens and user information
    return new AuthResponse(accessToken, refreshToken, "Bearer", 3600, userDto);
  }
}