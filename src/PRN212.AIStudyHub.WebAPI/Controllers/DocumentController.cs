using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN212.AIStudyHub.Application.DTOs.Document;
using PRN212.AIStudyHub.Application.Interfaces;
using PRN212.AIStudyHub.WebAPI.Models;

namespace PRN212.AIStudyHub.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("/api/v1/[controller]")]
public class DocumentController(IDocumentService documentService) : ControllerBase
{
  private readonly string[] _allowExtensions = { ".pdf", ".docx", ".doc", ".pptx", ".txt", ".txt", ".md" };
  private const long MaxFileSize = 25 * 1024 * 1024;

  [HttpPost("upload")]
  [Consumes("multipart/form-data")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> UploadAsync([FromForm] UploadDocumentRequest request, CancellationToken cancellationToken)
  {
    if (request.File == null || request.File.Length == 0)
      return BadRequest(new { Message = "Empty file" });

    if (request.File.Length > MaxFileSize)
      return BadRequest(new { Message = "File size cannot be over 25MB" });

    var fileExtension = Path.GetExtension(request.File.Name).ToLowerInvariant();
    if (!string.IsNullOrEmpty(fileExtension) || !_allowExtensions.Contains(fileExtension))
      return BadRequest(new { Message = "Invalid file extension" });

    try
    {
      var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      if (!Guid.TryParse(userIdStr, out Guid userId))
        return Unauthorized(new { Message = "Unauthorized" });

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
      return NotFound(new { ex.Message });
    }
    catch (Exception ex)
    {
      return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
    }
  }
}
