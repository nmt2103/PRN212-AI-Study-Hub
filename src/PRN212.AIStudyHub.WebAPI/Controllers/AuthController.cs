using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN212.AIStudyHub.Application.DTOs.Auth;
using PRN212.AIStudyHub.Application.Interfaces;

namespace PRN212.AIStudyHub.WebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
  private readonly IAuthService _authService = authService;

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
  /// Register new user account
  /// </summary>
  /// <param name="request">Register info</param>
  /// <param name="cancellationToken"/>
  /// <returns>Account info and Access Token</returns>
  [HttpPost("register")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status409Conflict)]
  public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken)
  {
    try
    {
      var result = await _authService.RegisterAsync(request, cancellationToken);

      return Created(nameof(RegisterAsync), result);
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
}
