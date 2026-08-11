using PRN212.AIStudyHub.Application.DTOs.Auth;

namespace PRN212.AIStudyHub.Application.Interfaces;

public interface IAuthService
{
  /// <summary>
  /// Authenticates a user based on their email and password.
  /// </summary>
  /// <param name="request">The login request containing email and plain-text password.</param>
  /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
  /// <returns>An <see cref="AuthResponse"/> containing the JWT access token, refresh token, and user profile information.</returns>
  /// <exception cref="Exception">Thrown when required input fields are missing.</exception>
  /// <exception cref="InvalidOperationException">Thrown when credentials are invalid or the account is currently inactive.</exception>
  Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

  /// <summary>
  /// Registers a new user account in the system and automatically authenticates them upon success.
  /// </summary>
  /// <param name="request">The registration request containing personal details and password.</param>
  /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
  /// <returns>An <see cref="AuthResponse"/> containing the JWT access token, refresh token, and the newly created user profile.</returns>
  /// <exception cref="Exception">Thrown when input validation fails, passwords do not match, or password is too short.</exception>
  /// <exception cref="InvalidOperationException">Thrown when the provided email is already registered in the system.</exception>
  Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

  /// <summary>
  /// Retrieves the profile information of the currently authenticated user by their unique identifier.
  /// </summary>
  /// <param name="userId">The unique identifier (GUID) of the user extracted from the JWT token.</param>
  /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
  /// <returns>A <see cref="UserDto"/> representing the user's profile details.</returns>
  /// <exception cref="Exception">Thrown when the user with the specified ID cannot be found in the database.</exception>
  Task<UserDto> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
