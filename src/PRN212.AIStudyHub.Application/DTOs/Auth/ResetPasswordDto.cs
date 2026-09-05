using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PRN212.AIStudyHub.Application.DTOs.Auth
{
  public class ResetPasswordDto
  {
	[Required(ErrorMessage = "Email cannot be empty.")]
	public string email { get; set; } = string.Empty;

	[Required(ErrorMessage = "OTP cannot be empty.")]
	public string otp { get; set; } = string.Empty;

	[Required(ErrorMessage = "New password cannot be empty.")]
	[MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
	public string newPassword { get; set; } = string.Empty;
  }
}
