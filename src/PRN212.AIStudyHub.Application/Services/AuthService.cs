using Microsoft.EntityFrameworkCore;
using PRN212.AIStudyHub.Application.DTOs.Auth;
using PRN212.AIStudyHub.Application.Interfaces;
using PRN212.AIStudyHub.Application.Interfaces.Security;
using PRN212.AIStudyHub.Domain.Entities;
using System.Security.Cryptography;
using PRN212.AIStudyHub.Application.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth;
using BCrypt.Net;

namespace PRN212.AIStudyHub.Application.Services;

public class AuthService(IAppDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IMemoryCache cache, IEmailService emailService, IConfiguration config) : IAuthService
{
	public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
	{
		// Validate the input parameters
		if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
		{
			throw new ArgumentException("Email and password are required");
		}

		// Validate format email
		if (!ValidationUtils.IsValidEmail(request.Email))
		{
			throw new ArgumentException("Invalid email format");
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

	public async Task<string> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
	{
		// Validate the input parameters
		if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.ConfirmPassword) || string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
		{
			throw new ArgumentException("All fields are required");
		}

		// Validate format for email
		if (!ValidationUtils.IsValidEmail(request.Email))
		{
			throw new ArgumentException("Invalid email format");
		}

		// Validate format for first name (allow Vietnamese)
		if (!ValidationUtils.IsValidName(request.FirstName))
		{
			throw new ArgumentException("Invalid first name format");
		}

		// Validate format for last name (allow Vietnamese)
		if (!ValidationUtils.IsValidName(request.LastName))
		{
			throw new ArgumentException("Invalid last name format");
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

		// Random OTP with 6 index
		var otp = new Random().Next(100000, 999999).ToString();

		// Save to cache memmory (expired in 5 minutes)
		var cacheKey = $"OTP_{request.Email}";
		var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
		cache.Set(cacheKey, new OtpCacheEntry(request, otp), cacheEntryOptions);

		// Send mail to user
		var subject = "Xác nhận đăng ký tài khoản AI Study Hub";
		var body = $"<h3>Chào {request.FirstName},</h3><p>Mã OTP xác nhận đăng ký tài khoản của bạn là: <strong>{otp}</strong></p><p>Mã này sẽ hết hạn sau 5 phút.</p>";
		await emailService.SendEmailAsync(request.Email, subject, body);

		return "Mã OTP đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư (bao gồm cả thư rác).";
	}

	public async Task<string> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default)
	{
		var cacheKey = $"OTP_{request.Email}";

		if (!cache.TryGetValue(cacheKey, out OtpCacheEntry? cachedEntry) || cachedEntry is null)
		{
			throw new InvalidOperationException("OTP code has expired");
		}

		if (cachedEntry.Otp != request.Otp)
		{
			throw new ArgumentException("OTP code incorrect");
		}

		var registerRequest = cachedEntry.Request;

		// Check database again to ensure thread-safety / state consistency
		var existingEmail = await context.AppUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Email == registerRequest.Email, cancellationToken);
		if (existingEmail is not null)
		{
			throw new InvalidOperationException("Email already exists");
		}

		var newUser = new AppUser
		{
			Email = registerRequest.Email,
			PasswordHash = passwordHasher.HashPassword(registerRequest.Password),
			FirstName = registerRequest.FirstName,
			LastName = registerRequest.LastName,
			Role = registerRequest.Role ?? "Student",
			IsActive = true,
		};

		context.AppUsers.Add(newUser);
		await context.SaveChangesAsync(cancellationToken);

		// Clear cache
		cache.Remove(cacheKey);

		return "Xác nhận OTP thành công! Tài khoản của bạn đã được tạo, vui lòng đăng nhập.";
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

	public async Task<GoogleLoginResult> GoogleLoginAsync(GoogleLoginRequest req)
	{
		var accessToken = req.GetTokenChecked();

		if (string.IsNullOrEmpty(accessToken))
		{
			throw new InvalidOperationException("Backend can't find the token");
		}

		string userEmail = "";
		string userName = "";
		string firstName = "";
		string lastName = "";

		if (accessToken.StartsWith("ya29"))
		{
			var httpClient = new HttpClient();
			var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={accessToken}");
			if (!response.IsSuccessStatusCode)
			{
				throw new UnauthorizedAccessException("Invalid Google Access Token");
			}
			var jsonResponse = await response.Content.ReadAsStringAsync();
			var docs = System.Text.Json.JsonDocument.Parse(jsonResponse);
			var root = docs.RootElement;

			userEmail = root.TryGetProperty("email", out var emailEl) ? emailEl.GetString() ?? "" : "";
			userName = root.TryGetProperty("name", out var nameEl) ? nameEl.GetString() ?? "" : "";
			firstName = root.TryGetProperty("given_name", out var givenNameEl) ? givenNameEl.GetString() ?? userName : userName;
			lastName = root.TryGetProperty("family_name", out var familyNameEl) ? familyNameEl.GetString() ?? "" : "";
		}
		else
		{
			var clientId = config["Google:ClientId"];
			var settings = new GoogleJsonWebSignature.ValidationSettings()
			{
				Audience = new List<string>() { clientId ?? string.Empty }
			};

			var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken, settings);
			userEmail = payload.Email;
			userName = payload.Name;
			firstName = payload.GivenName ?? userName;
			lastName = payload.FamilyName ?? "";
		}

		var userInDb = await context.AppUsers.FirstOrDefaultAsync(u => u.Email == userEmail);

		if (userInDb != null)
		{
			// User exists, return standard auth response
			return new GoogleLoginResult
			{
				IsNewUser = false,
				AuthResponse = await GenerateAuthResponseAsync(userInDb, default)
			};
		}
		else
		{
			// New user, generate temporary token
			var tempToken = jwtTokenGenerator.GenerateTemporaryToken(userEmail, firstName, lastName);
			return new GoogleLoginResult
			{
				IsNewUser = true,
				TemporaryToken = tempToken,
				Message = "Please choose your role to complete registation."
			};
		}
	}

	// Khi đăng nhập bằng google sẽ cho chọn role nữa sau đó mới lưu vào database thông tin từ google (gồm email, tên, role)
	public async Task<AuthResponse> CompleteGoogleRegistrationAsync(CompleteGoogleRegistrationRequest request, string tempToken)
	{
		var principal = jwtTokenGenerator.ValidateTemporaryToken(tempToken);
		if (principal == null)
		{
			throw new UnauthorizedAccessException("Temporary token is invalid or expired.");
		}

		var email = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
		var firstName = principal.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value ?? "";
		var lastName = principal.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value ?? "";

		if (string.IsNullOrEmpty(email))
		{
			throw new InvalidOperationException("Invalid token payload.");
		}

		if (request.Role != "Student" && request.Role != "Lecturer")
		{
			throw new ArgumentException("Invalid role.");
		}

		// Double check DB
		var userInDb = await context.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
		if (userInDb != null)
		{
			return await GenerateAuthResponseAsync(userInDb, default);
		}

		string randomDummyPassword = Guid.NewGuid().ToString();
		string hashedDummyPassword = BCrypt.Net.BCrypt.HashPassword(randomDummyPassword);

		var newUser = new AppUser
		{
			Email = email,
			FirstName = firstName,
			LastName = lastName,
			PasswordHash = hashedDummyPassword,
			Role = request.Role,
			IsActive = true
		};

		context.AppUsers.Add(newUser);
		await context.SaveChangesAsync(default);

		return await GenerateAuthResponseAsync(newUser, default);
	}

	public async Task<string> ForgotPassword(ForgotPasswordRequestDto request)
	{
		var userInDb = await context.AppUsers.FirstOrDefaultAsync(u => u.Email == request.email);
		if (userInDb == null)
		{
			throw new KeyNotFoundException("Account does not exist");
		}

		string otp = new Random().Next(100000, 999999).ToString();

		var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
		cache.Set($"OTP_{request.email}", otp, cacheOptions);

		if (!string.IsNullOrEmpty(userInDb.Email) && ValidationUtils.IsValidEmail(userInDb.Email))
		{
			try
			{
				string subject = "eParking - Reset Password OTP Verification";
				string body = $@"
                        <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 8px; max-width: 600px;'>
                            <h2 style='color: #2563eb; text-align: center;'>eParking Verification Code</h2>
                            <p>Hello,</p>
                            <p>We received a request to reset your password. Please use the verification code below to proceed:</p>
                            <div style='text-align: center; margin: 30px 0;'>
                                <span style='font-size: 28px; font-weight: bold; letter-spacing: 4px; background-color: #f3f4f6; padding: 10px 24px; border-radius: 6px; border: 1px solid #e5e7eb;'>{otp}</span>
                            </div>
                            <p>This code is only valid for <strong>5 minutes</strong>. If you did not request this, please ignore this email.</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;' />
                            <p style='font-size: 12px; color: #666; text-align: center;'>This is an automated message, please do not reply directly to this email.</p>
                        </div>";
				await emailService.SendEmailAsync(userInDb.Email, subject, body);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[EMAIL_ERROR] Failed to send email to {userInDb.Email}: {ex.Message}");
			}
		}
		return "Vui lòng nhập mã OTP (đã gửi qua mail) để thay đổi mật khẩu";
	}

	public async Task<string> ResetPassword(ResetPasswordDto request)
	{
		if (!cache.TryGetValue($"OTP_{request.email}", out string? savedOtp))
		{
			throw new KeyNotFoundException("OTP has expired (over 5 minutes) or has not been requested");
		}

		if (savedOtp != request.otp)
		{
			throw new KeyNotFoundException("Invalid OTP");
		}

		var userInDb = await context.AppUsers.FirstOrDefaultAsync(u => u.Email == request.email);
		if (userInDb == null)
		{
			throw new KeyNotFoundException("Account does not exist");
		}

		userInDb.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.newPassword);
		await context.SaveChangesAsync();
		cache.Remove($"OTP_{request.email}");
		return "Mật khẩu đã được cập nhật thành công";
	}
}
