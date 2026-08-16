using Microsoft.EntityFrameworkCore;
using PRN212.AIStudyHub.Application.DTOs.Document;
using PRN212.AIStudyHub.Application.Interfaces;
using PRN212.AIStudyHub.Application.Services.Cloud;
using PRN212.AIStudyHub.Domain.Entities;

namespace PRN212.AIStudyHub.Application.Services;

public class DocumentService(IAppDbContext context, ICloudStorageService cloudStorageService) : IDocumentService
{
  public async Task<DocumentResponseDto> UploadDocumentAsync(UploadDocumentCommand request, Guid userId, CancellationToken cancellationToken = default)
  {
    var isSubjectExist = await context.Subjects.AsNoTracking().AnyAsync(subject => subject.Id == request.SubjectId, cancellationToken);

    if (!isSubjectExist)
      throw new InvalidOperationException("Invalid subject");

    var cloudUploadResult = await cloudStorageService.UploadRawFileAsync(request.FileStream, request.FileName, cancellationToken);

    var newDocument = new Document
    {
      UserId = userId,
      SubjectId = request.SubjectId,
      Title = request.Title,
      FileName = request.FileName,
      StoragePath = cloudUploadResult.SecureUrl,
      FileSize = request.FileSize,
      FileExtension = Path.GetExtension(request.FileName),
      ContentType = request.ContentType,
      IsCloudStored = true,
      CloudPublicId = cloudUploadResult.PublicId,
      IsPublic = request.IsPublic,
    };

    context.Documents.Add(newDocument);
    await context.SaveChangesAsync(cancellationToken);

    return new DocumentResponseDto(newDocument.Id, newDocument.Title, newDocument.FileName, newDocument.StoragePath, newDocument.CloudPublicId, newDocument.IsCloudStored, newDocument.FileSize, newDocument.FileExtension, newDocument.ContentType, newDocument.UploadedAt, newDocument.IsPublic, newDocument.SubjectId);
  }
}