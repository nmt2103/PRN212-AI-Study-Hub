using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN212.AIStudyHub.Application.DTOs.Common;
using PRN212.AIStudyHub.Application.DTOs.Subject;
using PRN212.AIStudyHub.Application.Interfaces.Security;

namespace PRN212.AIStudyHub.WebAPI.Controllers;

[Authorize]
[Route("api/v1/subjects")]
public class SubjectController(ISubjectService subjectService) : BaseApiController
{
  /// <summary>
  /// Lấy toàn bộ danh mục môn học trong hệ thống
  /// </summary>
  [HttpGet]
  [ProducesResponseType(typeof(ApiResponse<List<SubjectDto>>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetAllSubjects(CancellationToken cancellationToken)
  {
    var result = await subjectService.GetAllSubjects(cancellationToken);
    return Ok(ApiResponse<List<SubjectDto>>.SuccessResponse(result, "Fetched all subjects successfully."));
  }

  /// <summary>
  /// Tạo mới môn học
  /// </summary>
  [HttpPost]
  [ProducesResponseType(typeof(ApiResponse<SubjectDto>), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> CreateSubject(
    [FromBody] CreateSubjectRequest request,
    CancellationToken cancellationToken)
  {
    var result = await subjectService.CreateSubject(request, cancellationToken);
    return StatusCode(StatusCodes.Status201Created,
      ApiResponse<SubjectDto>.SuccessResponse(result, "Subject created successfully."));
  }
}
