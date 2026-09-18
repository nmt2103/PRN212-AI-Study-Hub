using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PRN212.AIStudyHub.Application.DTOs.Subject
{
	public record CreateSubjectRequest
	(
		[Required(ErrorMessage = "Subject name cannot be emptied")]
		string Name,
		string? Description
	);
}
