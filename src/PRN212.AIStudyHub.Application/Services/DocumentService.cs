using Microsoft.EntityFrameworkCore;
using PRN212.AIStudyHub.Application.DTOs.Common;
using PRN212.AIStudyHub.Application.DTOs.Document;
using PRN212.AIStudyHub.Application.Exceptions;
using PRN212.AIStudyHub.Application.Interfaces;
using PRN212.AIStudyHub.Application.Services.Cloud;
using PRN212.AIStudyHub.Domain.Entities;

namespace PRN212.AIStudyHub.Application.Services;

public class DocumentService(IAppDbContext context, ICloudStorageService cloudStorageService) : IDocumentService
{
  public async Task<DocumentResponseDto> UploadDocument(
	  UploadDocumentCommand request,
	  Guid userId,
	  CancellationToken cancellationToken = default)
  {
	var isSubjectExist = await context.Subjects.AsNoTracking()
			.AnyAsync(subject => subject.Id == request.SubjectId, cancellationToken);

	if (!isSubjectExist)
	  throw new NotFoundException($"Subject with ID '{request.SubjectId}' was not found.");

	var cloudUploadResult = await cloudStorageService.UploadRawFile(
			request.FileStream,
			request.FileName,
			cancellationToken);

	var newDocument = new Document
	{
	  Id = Guid.CreateVersion7(),
	  UserId = userId,
	  SubjectId = request.SubjectId,
	  Title = request.Title,
	  FileName = request.FileName,
	  StoragePath = cloudUploadResult.SecureUrl,
	  FileSize = request.FileSize,
	  FileExtension = Path.GetExtension(request.FileName),
	  ContentType = request.ContentType,
	  UploadedAt = DateTime.UtcNow,
	  IsCloudStored = true,
	  CloudPublicId = cloudUploadResult.PublicId,
	  IsPublic = request.IsPublic,
	  ProcessingStatus = "Pending",
	  IsDeleted = false
	};

	context.Documents.Add(newDocument);
	await context.SaveChangesAsync(cancellationToken);

	return new DocumentResponseDto(
		newDocument.Id,
		newDocument.Title,
		newDocument.FileName,
		newDocument.StoragePath,
		newDocument.CloudPublicId,
		newDocument.IsCloudStored,
		newDocument.FileSize,
		newDocument.FileExtension,
		newDocument.ContentType,
		newDocument.UploadedAt,
		newDocument.IsPublic,
		newDocument.SubjectId);
  }

  public async Task<List<DocumentItemDto>> GetMyDocuments(
	  Guid userId,
	  Guid? subjectId = null,
	  CancellationToken cancellationToken = default)
  {
	var query = context.Documents.AsNoTracking()
			.Include(d => d.Subject)
			.Where(d => d.UserId == userId && d.IsDeleted == false);

	if (subjectId.HasValue)
	{
	  query = query.Where(d => d.SubjectId == subjectId.Value);
	}

	var result = await query
		.OrderByDescending(d => d.UploadedAt)
		.Select(d => new DocumentItemDto(
			d.Id,
			d.Title,
			d.FileName,
			d.SubjectId,
			d.Subject.Name,
			d.UploadedAt,
			d.ProcessingStatus,
			d.IsPublic))
		.ToListAsync(cancellationToken);

	return result;
  }

  public async Task<DocumentResponseDto> GetDocumentById(
	  Guid id,
	  Guid userId,
	  CancellationToken cancellationToken = default)
  {
	var docs = await context.Documents.AsNoTracking()
			.FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false, cancellationToken);

	if (docs == null)
	{
	  throw new NotFoundException($"Document with ID '{id}' was not found.");
	}

	if (docs.UserId != userId && docs.IsPublic == false)
	{
	  throw new ForbiddenException("You do not have permission to view this private document.");
	}

	return new DocumentResponseDto(
		docs.Id,
		docs.Title,
		docs.FileName,
		docs.StoragePath,
		docs.CloudPublicId,
		docs.IsCloudStored,
		docs.FileSize,
		docs.FileExtension,
		docs.ContentType,
		docs.UploadedAt,
		docs.IsPublic,
		docs.SubjectId);
  }

  public async Task<bool> DeleteDocument(
	  Guid id,
	  Guid userId,
	  CancellationToken cancellationToken = default)
  {
	var doc = await context.Documents
			.FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false, cancellationToken);

	if (doc == null)
	{
	  throw new NotFoundException($"Document with ID '{id}' was not found.");
	}

	if (doc.UserId != userId)
	{
	  throw new ForbiddenException("Only the owner can delete this document.");
	}

	doc.IsDeleted = true;
	doc.DeletedAt = DateTime.UtcNow;

	await context.SaveChangesAsync(cancellationToken);
	return true;
  }

  public async Task<DocumentResponseDto> UpdateDocument(
	  Guid id,
	  Guid userId,
	  UpdateDocumentRequest request,
	  CancellationToken cancellationToken = default)
  {
	var docs = await context.Documents
			.FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false, cancellationToken);

	if (docs == null)
	{
	  throw new NotFoundException($"Document with ID '{id}' was not found.");
	}

	if (docs.UserId != userId)
	{
	  throw new ForbiddenException("Only the owner can edit this document.");
	}

	var subjectExist = await context.Subjects.AnyAsync(s => s.Id == request.SubjectId, cancellationToken);
	if (!subjectExist)
	{
	  throw new NotFoundException($"Subject with ID '{request.SubjectId}' was not found.");
	}

	docs.Title = request.Title.Trim();
	docs.SubjectId = request.SubjectId;
	docs.IsPublic = request.IsPublic;

	await context.SaveChangesAsync(cancellationToken);

	return new DocumentResponseDto(
		docs.Id,
		docs.Title,
		docs.FileName,
		docs.StoragePath,
		docs.CloudPublicId,
		docs.IsCloudStored,
		docs.FileSize,
		docs.FileExtension,
		docs.ContentType,
		docs.UploadedAt,
		docs.IsPublic,
		docs.SubjectId);
  }

  public async Task<DocumentResponseDto> UpdateDocumentSubject(
	  Guid documentId,
	  Guid currentUserId,
	  bool isAdmin,
	  UpdateDocumentSubjectRequest request,
	  CancellationToken cancellationToken = default)
  {
	var document = await context.Documents
		.FirstOrDefaultAsync(doc => doc.Id == documentId && !doc.IsDeleted, cancellationToken);

	if (document is null)
	{
	  throw new NotFoundException($"Document with ID '{documentId}' was not found");
	}

	if (document.UserId != currentUserId && !isAdmin)
	{
	  throw new ForbiddenException("Only the owner can edit this document.");
	}

	var subjectExist = await context.Subjects
		.AnyAsync(subject => subject.Id == request.SubjectId, cancellationToken);
	if (!subjectExist)
	{
	  throw new NotFoundException($"Subject with ID '{request.SubjectId}' was not found.");
	}

	document.SubjectId = request.SubjectId;
	await context.SaveChangesAsync(cancellationToken);

	return new DocumentResponseDto(
		document.Id,
		document.Title,
		document.FileName,
		document.StoragePath,
		document.CloudPublicId,
		document.IsCloudStored,
		document.FileSize,
		document.FileExtension,
		document.ContentType,
		document.UploadedAt,
		document.IsPublic,
		document.SubjectId);
  }

  public async Task<PagedResult<DocumentResponseDto>> GetDocumentsBySubject(
	  Guid subjectId,
	  Guid currentUserId,
	  int pageNumber,
	  int pageSize,
	  CancellationToken cancellationToken = default)
  {
	var isSubjectExist = await context.Subjects
		.AnyAsync(subject => subject.Id == subjectId, cancellationToken);

	if (!isSubjectExist)
	{
	  throw new NotFoundException($"Subject with ID '{subjectId}' was not found.");
	}

	var query = context.Documents.AsNoTracking()
		.Where(doc => doc.SubjectId == subjectId && !doc.IsDeleted
			&& (doc.UserId == currentUserId || doc.IsPublic));

	int totalCount = await query.CountAsync(cancellationToken);

	var items = await query.OrderByDescending(doc => doc.UploadedAt)
		.Skip((pageNumber - 1) * pageSize)
		.Take(pageSize)
		.Select(doc => new DocumentResponseDto(
			doc.Id,
			doc.Title,
			doc.FileName,
			doc.StoragePath,
			doc.CloudPublicId,
			doc.IsCloudStored,
			doc.FileSize,
			doc.FileExtension,
			doc.ContentType,
			doc.UploadedAt,
			doc.IsPublic,
			doc.SubjectId))
		.ToListAsync(cancellationToken);

	return PagedResult<DocumentResponseDto>.Create(items, totalCount, pageNumber, pageSize);
  }
}
