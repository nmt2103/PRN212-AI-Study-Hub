using PRN212.AIStudyHub.Application.DTOs.Subject;
using System;
using System.Collections.Generic;
using System.Text;

namespace PRN212.AIStudyHub.Application.Interfaces.Security
{
  public interface ISubjectService
  {
		Task<List<SubjectDto>> GetAllSubjectsAsync(CancellationToken cancellationToken = default);
		Task<SubjectDto> CreateSubjectAsync(CreateSubjectRequest request, CancellationToken cancellationToken = default);
  }
}
