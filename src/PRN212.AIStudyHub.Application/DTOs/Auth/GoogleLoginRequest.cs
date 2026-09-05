using System;
using System.Collections.Generic;
using System.Text;

namespace PRN212.AIStudyHub.Application.DTOs.Auth
{
  public class GoogleLoginRequest
  {
	public string? AccessToken { get; set; }

	public string GetTokenChecked()
	{
	  if (!string.IsNullOrEmpty(AccessToken)) return AccessToken;
	  return string.Empty;
	}
  }
}
