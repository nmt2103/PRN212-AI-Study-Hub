using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN212.AIStudyHub.Application.DTOs.Document;
using PRN212.AIStudyHub.Application.Exceptions;
using PRN212.AIStudyHub.Application.Interfaces;
using PRN212.AIStudyHub.WebAPI.Models;

namespace PRN212.AIStudyHub.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("/api/v1/documents")]
public class DocumentController(IDocumentService documentService, ILogger<DocumentController> logger) : ControllerBase
{
  private readonly string[] _allowExtensions = { ".pdf", ".docx", ".doc", ".pptx", ".txt", ".md" };
  private const long MaxFileSize = 25 * 1024 * 1024;

  /// <summary>
  /// Tải lên tài liệu học tập và lưu trữ trực tiếp lên Cloudinary
  /// </summary>
  /// <param name="request">Dữ liệu tài liệu và tệp đính kèm (multipart/form-data)</param>
  /// <param name="cancellationToken"></param>
  /// <returns>Thông tin tài liệu đã lưu thành công</returns>
  [HttpPost("upload")]
  [Consumes("multipart/form-data")]
  [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status502BadGateway)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> UploadAsync(
		[FromForm] UploadDocumentRequest request,
		CancellationToken cancellationToken)
  {
	var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
	if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
	  return Unauthorized(new { Message = "Unauthorized" });

	if (request.File == null || request.File.Length == 0)
	  return BadRequest(new { Message = "Empty file" });

	if (request.File.Length > MaxFileSize)
	  return BadRequest(new { Message = "File size cannot be over 25MB" });

	var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
	if (string.IsNullOrEmpty(fileExtension) || !_allowExtensions.Contains(fileExtension))
	  return BadRequest(new { Message = "Invalid file extension" });

	try
	{
	  using var fileStream = request.File.OpenReadStream();

	  var command = new UploadDocumentCommand(
	  FileStream: fileStream,
	  FileName: request.File.FileName,
	  ContentType: request.File.ContentType,
	  FileSize: request.File.Length,
	  Title: request.Title,
	  SubjectId: request.SubjectId,
	  IsPublic: request.IsPublic
	  );

	  var result = await documentService.UploadDocumentAsync(command, userId, cancellationToken);

	  return StatusCode(StatusCodes.Status201Created, result);
	}
	catch (InvalidOperationException ex)
	{
	  logger.LogWarning(ex, "Subject not found: {SubjectId}", request.SubjectId);
	  return NotFound(new { ex.Message });
	}
	catch (CloudStorageException ex)
	{
	  logger.LogError(ex, "Cloudinary upload failed for file: {FileName}", request.File.FileName);
	  return StatusCode(StatusCodes.Status502BadGateway,
				new { Message = "Cloud storage service error", Detail = ex.Message });
	}
	catch (Exception ex)
	{
	  logger.LogError(ex, "Unexpected error uploading file: {FileName}", request.File.FileName);
	  return StatusCode(StatusCodes.Status500InternalServerError,
				new { Message = "An unexpected error occurred", Detail = ex.Message });
	}
  }

  [HttpGet("get")]
  [ProducesResponseType(typeof(List<DocumentItemDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> GetMyDocument(
		[FromQuery] Guid? subjectId,
		CancellationToken cancellationToken)
  {
	var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
	if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
	{
	  return Unauthorized(new { message = "Unauthorized" });
	}
	try
	{
	  var result = await documentService.GetDocumentAsync(userId, subjectId, cancellationToken);
	  return Ok(result);
	}
	catch (Exception ex)
	{
	  logger.LogError(ex, "Failed to get list of document from user {userId}", userId);
	  return StatusCode(StatusCodes.Status500InternalServerError,
				new { Message = "An unexpected error occurred", Detail = ex.Message });
	}
  }

  [HttpGet("{id}")]
  [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<IActionResult> GetDocumentById([FromRoute] Guid id, CancellationToken cancellationToken)
  {
	var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
	if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
	{
	  return Unauthorized(new { message = "Unauthorized" });
	}

	try
	{
	  var result = await documentService.GetDocumentDetailsAsync(id, userId, cancellationToken);
	  return Ok(result);
	}
	catch (KeyNotFoundException ex)
	{
	  return NotFound(new { message = ex.Message });
	}
	catch (UnauthorizedAccessException ex)
	{
	  return StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
	}
	catch (Exception ex)
	{
	  logger.LogError(ex, "An unexpected error getting document {DocId}", id);
	  return StatusCode(StatusCodes.Status500InternalServerError,
				new { message = "An unexpected error occurred", Detail = ex.Message });
	}
  }

  [HttpPut("{id}")]
  [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateDocument(
		[FromRoute] Guid id,
		[FromBody] UpdateDocumentRequest request,
		CancellationToken cancellationToken)
  {
	var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
	if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
	{
	  return Unauthorized(new { Message = "Unauthorized" });
	}

	try
	{
	  var result = await documentService.UpdateDocumentAsync(id, userId, request, cancellationToken);
	  return Ok(result);
	}
	catch (KeyNotFoundException ex)
	{
	  return NotFound(new { Message = ex.Message });
	}
	catch (UnauthorizedAccessException ex)
	{
	  return StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
	}
	catch (InvalidOperationException ex)
	{
	  return BadRequest(new { Message = ex.Message });
	}
	catch (Exception ex)
	{
	  logger.LogError(ex, "An unexpected error updating document {DocId}", id);
	  return StatusCode(StatusCodes.Status500InternalServerError,
				new { Message = "An unexpected error occurred", Detail = ex.Message });
	}
  }

  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteDocument([FromRoute] Guid id, CancellationToken cancellationToken)
  {
	var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
	if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
	{
	  return Unauthorized(new { Message = "Unauthorized" });
	}

	try
	{
	  await documentService.DeleteDocumentAsync(id, userId, cancellationToken);
	  return NoContent();
	}
	catch (KeyNotFoundException ex)
	{
	  return NotFound(new { Message = ex.Message });
	}
	catch (UnauthorizedAccessException ex)
	{
	  return StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
	}
	catch (Exception ex)
	{
	  logger.LogError(ex, "An unexpected error deleting document {DocId}", id);
	  return StatusCode(StatusCodes.Status500InternalServerError,
				new { Message = "An unexpected error occurred", Detail = ex.Message });
	}
  }
}
