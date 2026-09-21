using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN212.AIStudyHub.Application.DTOs.Common;
using PRN212.AIStudyHub.Application.DTOs.Document;
using PRN212.AIStudyHub.Application.Exceptions;
using PRN212.AIStudyHub.Application.Interfaces;
using PRN212.AIStudyHub.WebAPI.Models;

namespace PRN212.AIStudyHub.WebAPI.Controllers;

[Authorize]
[Route("api/v1/documents")]
public class DocumentController(IDocumentService documentService) : BaseApiController
{
  private static readonly string[] AllowedExtensions = [".pdf", ".docx", ".doc", ".pptx", ".txt", ".md"];
  private const long MaxFileSize = 25 * 1024 * 1024; // 25MB

  /// <summary>
  /// Tải lên tài liệu học tập và lưu trữ lên Cloudinary
  /// </summary>
  [HttpPost("upload")]
  [Consumes("multipart/form-data")]
  [ProducesResponseType(typeof(ApiResponse<DocumentResponseDto>), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status502BadGateway)]
  public async Task<IActionResult> UploadAsync(
		[FromForm] UploadDocumentRequest request,
		CancellationToken cancellationToken)
  {
	if (request.File == null || request.File.Length == 0)
	  throw new BadRequestException("Uploaded file cannot be empty.");

	if (request.File.Length > MaxFileSize)
	  throw new BadRequestException("File size cannot exceed 25MB.");

	var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
	if (string.IsNullOrEmpty(fileExtension) || !AllowedExtensions.Contains(fileExtension))
	  throw new BadRequestException($"File extension '{fileExtension}' is not supported. Allowed extensions: {string.Join(", ", AllowedExtensions)}");

	using var fileStream = request.File.OpenReadStream();

	var command = new UploadDocumentCommand(
		FileStream: fileStream,
		FileName: request.File.FileName,
		ContentType: request.File.ContentType,
		FileSize: request.File.Length,
		Title: request.Title,
		SubjectId: request.SubjectId,
		IsPublic: request.IsPublic);

	var result = await documentService.UploadDocumentAsync(command, CurrentUserId, cancellationToken);

	return StatusCode(StatusCodes.Status201Created,
		ApiResponse<DocumentResponseDto>.SuccessResponse(result, "Document uploaded successfully."));
  }

  /// <summary>
  /// Lấy danh sách tài liệu cá nhân (hỗ trợ lọc theo môn học)
  /// </summary>
  [HttpGet]
  [HttpGet("get")] // Hỗ trợ tương thích ngược
  [ProducesResponseType(typeof(ApiResponse<List<DocumentItemDto>>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> GetMyDocuments(
		[FromQuery] Guid? subjectId,
		CancellationToken cancellationToken)
  {
	var result = await documentService.GetDocumentAsync(CurrentUserId, subjectId, cancellationToken);
	return Ok(ApiResponse<List<DocumentItemDto>>.SuccessResponse(result, "Fetched documents successfully."));
  }

  /// <summary>
  /// Xem chi tiết thông tin tài liệu
  /// </summary>
  [HttpGet("{id:guid}")]
  [ProducesResponseType(typeof(ApiResponse<DocumentResponseDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetDocumentById(
		[FromRoute] Guid id,
		CancellationToken cancellationToken)
  {
	var result = await documentService.GetDocumentDetailsAsync(id, CurrentUserId, cancellationToken);
	return Ok(ApiResponse<DocumentResponseDto>.SuccessResponse(result, "Fetched document details successfully."));
  }

  /// <summary>
  /// Cập nhật thông tin tài liệu (chỉ áp dụng cho chủ sở hữu)
  /// </summary>
  [HttpPut("{id:guid}")]
  [ProducesResponseType(typeof(ApiResponse<DocumentResponseDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateDocument(
		[FromRoute] Guid id,
		[FromBody] UpdateDocumentRequest request,
		CancellationToken cancellationToken)
  {
	var result = await documentService.UpdateDocumentAsync(id, CurrentUserId, request, cancellationToken);
	return Ok(ApiResponse<DocumentResponseDto>.SuccessResponse(result, "Document updated successfully."));
  }

  /// <summary>
  /// Xóa mềm tài liệu
  /// </summary>
  [HttpDelete("{id:guid}")]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
  [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteDocument(
		[FromRoute] Guid id,
		CancellationToken cancellationToken)
  {
	await documentService.DeleteDocumentAsync(id, CurrentUserId, cancellationToken);
	return Ok(ApiResponse.SuccessResponse("Document deleted successfully."));
  }
}
