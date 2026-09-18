using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN212.AIStudyHub.Application.DTOs.Subject;
using PRN212.AIStudyHub.Application.Interfaces.Security;

namespace PRN212.AIStudyHub.WebAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/v1/subjects")]
  public class SubjectController(ISubjectService subjectService, ILogger<SubjectController> logger) : ControllerBase
  {
		[HttpGet]
		[ProducesResponseType(typeof(List<SubjectDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAllSubjects (CancellationToken cancellationToken)
		{
			var result = await subjectService.GetAllSubjectsAsync(cancellationToken);
			return Ok(result);
		}

		[HttpPost]
		[ProducesResponseType(typeof(SubjectDto), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectRequest request, CancellationToken cancellationToken)
		{
			try
			{
				var result = await subjectService.CreateSubjectAsync(request, cancellationToken);
				return StatusCode(StatusCodes.Status201Created, result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new {message = ex.Message});
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An unexpected error while creating new subject: {SubjectName}", request.Name);
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred", Detail = ex.Message });
			}
		}
  }
}
