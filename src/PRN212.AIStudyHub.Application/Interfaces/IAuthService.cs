using PRN212.AIStudyHub.Application.DTOs.Auth;

namespace PRN212.AIStudyHub.Application.Interfaces;

public interface IAuthService
{
  Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
  Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
  Task<UserDto> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
