using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PRN212.AIStudyHub.Application.DTOs.Auth;
using PRN212.AIStudyHub.Application.Interfaces;

namespace PRN212.AIStudyHub.WebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IAuthService authService, IMemoryCache memoryCache) : ControllerBase
{
  private readonly IAuthService _authService = authService;
  private readonly IMemoryCache _memoryCache = memoryCache;


  /// <summary>
  /// Register new user account
  /// </summary>
  /// <param name="request">Register info</param>
  /// <param name="cancellationToken"/>
  /// <returns>Account info and Access Token</returns>
  [HttpPost("register")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken)
  {
	try
	{
	  var result = await _authService.RegisterAsync(request, cancellationToken);
	  return Ok(new { message = result });
	}
	catch (InvalidOperationException ex)
	{
	  return Conflict(new { ex.Message });
	}
	catch (ArgumentException ex)
	{
	  return BadRequest(new { ex.Message });
	}
  }


  /// <summary>
  /// Login user account
  /// </summary>
  /// <param name="request"> Login info </param>
  /// <paramref name="cancellationToken"/>
  /// <returns>Account info and Access token</returns>
  [HttpPost("login")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
  {
	try
	{
	  var result = await _authService.LoginAsync(request, cancellationToken);

	  return Ok(result);
	}
	catch (InvalidOperationException ex)
	{
	  return Unauthorized(new { ex.Message });
	}
	catch (ArgumentException ex)
	{
	  return BadRequest(new { ex.Message });
	}
  }


  /// <summary>
  /// Verify OTP to complete registration
  /// </summary>
  [HttpPost("verify-otp")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> VerifyOtpAsync([FromBody] VerifyOtpRequest request, CancellationToken cancellationToken)
  {
	try
	{
	  var result = await _authService.VerifyOtpAsync(request, cancellationToken);
	  return Ok(new { message = result });
	}
	catch (InvalidOperationException ex)
	{
	  return BadRequest(new { ex.Message });
	}
	catch (ArgumentException ex)
	{
	  return BadRequest(new { ex.Message });
	}
  }

  [HttpPost("google-login")]
  public async Task<IActionResult> GoogleLoginAsync([FromBody] GoogleLoginRequest request)
  {
	try
	{
	  var result = await _authService.GoogleLoginAsync(request);
	  if (result.IsNewUser)
	  {
		return StatusCode(202, new
		{
		  isNewUser = true,
		  temporaryToken = result.TemporaryToken,
		  message = result.Message
		});
	  }
	  return Ok(result.AuthResponse);
	}
	catch (UnauthorizedAccessException ex)
	{
	  return Unauthorized(new { ex.Message });
	}
	catch (Exception ex)
	{
	  return BadRequest(new { ex.Message });
	}
  }


  [HttpPost("complete-google-registration")]
  public async Task<IActionResult> CompleteGoogleRegistration([FromBody] CompleteGoogleRegistrationRequest request)
  {
	try
	{
	  // Lấy token từ header "Authorization: Bearer <token>"
	  var authHeader = Request.Headers["Authorization"].ToString();
	  if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
	  {
		return Unauthorized(new { Message = "Missing or invalid Authorization header." });
	  }

	  var token = authHeader.Substring("Bearer ".Length).Trim();

	  var result = await _authService.CompleteGoogleRegistrationAsync(request, token);
	  return Ok(result);
	}
	catch (UnauthorizedAccessException ex)
	{
	  return Unauthorized(new { ex.Message });
	}
	catch (Exception ex)
	{
	  return BadRequest(new { ex.Message });
	}
  }

  [HttpPost("logout")]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> LogoutAsync()
  {
	var authTokenHeader = Request.Headers["Authorization"].FirstOrDefault();
	if (string.IsNullOrEmpty(authTokenHeader) || !authTokenHeader.StartsWith("Bearer "))
	{
	  return BadRequest(new
	  {
		success = false,
		error_code = "INVALID_TOKEN",
		message = "Invalid token format"
	  });
	}
	var authToken = authTokenHeader.Substring("Bearer ".Length).Trim();
	var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
	_memoryCache.Set($"Blacklist_{authToken}", true, cacheOptions);

	return Ok(new
	{
	  success = true,
	  message = "Logout Succesfully."
	});
  }



  /// <summary>
  /// Get current user account profile
  /// </summary>
  /// <param name="cancellationToken"/>
  /// <returns>Account profile info</returns>
  [HttpGet("me")]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetCurrentUserAsync(CancellationToken cancellationToken)
  {
	try
	{
	  var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

	  if (userIdClaim is null)
		return Unauthorized(new { Message = "Unauthorized" });

	  if (!Guid.TryParse(userIdClaim, out var userId))
		return Unauthorized(new { Message = $"Invalid token payload. Claim value is: {userIdClaim}" });

	  var result = await _authService.GetCurrentUserAsync(userId, cancellationToken);

	  return Ok(result);
	}
	catch (KeyNotFoundException ex)
	{
	  return NotFound(new { ex.Message });
	}
	catch (Exception ex)
	{
	  return BadRequest(new { ex.Message });
	}
  }

  [HttpPost("forgot-password")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
  {
	try
	{
	  var result = await _authService.ForgotPassword(request);
	  return Ok(new { message = result });
	}
	catch (KeyNotFoundException ex)
	{
	  return NotFound(new { error = ex.Message });
	}
	catch (Exception ex)
	{
	  return BadRequest(new { error = ex.Message });
	}
  }

  [HttpPost("reset-password")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
  {
	try
	{
	  var result = await _authService.ResetPassword(request);
	  return Ok(new { message = result });
	}
	catch (KeyNotFoundException ex)
	{
	  return NotFound(new { error = ex.Message });
	}
	catch (Exception ex)
	{
	  return BadRequest(new { error = ex.Message });
	}
  }
}
