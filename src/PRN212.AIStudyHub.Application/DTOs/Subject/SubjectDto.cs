using System;
using System.Collections.Generic;
using System.Text;

namespace PRN212.AIStudyHub.Application.DTOs.Subject
{
	public record SubjectDto(Guid id, string Name, string? Description, DateTime CreatedAt);
}
