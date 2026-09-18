using Microsoft.EntityFrameworkCore;
using PRN212.AIStudyHub.Application.DTOs.Subject;
using PRN212.AIStudyHub.Application.Interfaces;
using PRN212.AIStudyHub.Application.Interfaces.Security;
using PRN212.AIStudyHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PRN212.AIStudyHub.Application.Services
{
	public class SubjectService(IAppDbContext context) : ISubjectService
	{
		public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectRequest request, CancellationToken cancellationToken = default)
		{
			var isExist = await context.Subjects.AnyAsync(s => s.Name.ToLower() == request.Name.ToLower(), cancellationToken);

			if (isExist)
			{
				throw new ArgumentException("This subject have existed in system");
			}

			var newSubject = new Subject
			{
				Id = Guid.CreateVersion7(),
				Name = request.Name,
				Description = request.Description,
				CreatedAt = DateTime.UtcNow,
			};
			await context.SaveChangesAsync(cancellationToken);

			return new SubjectDto(newSubject.Id, newSubject.Name, newSubject.Description, newSubject.CreatedAt);
		}

		public async Task<List<SubjectDto>> GetAllSubjectsAsync(CancellationToken cancellationToken = default)
		{
			return await context.Subjects.AsNoTracking().OrderBy(s => s.Name).Select(s => new SubjectDto(s.Id, s.Name, s.Description, s.CreatedAt)).ToListAsync(cancellationToken)
		}
	}
}
