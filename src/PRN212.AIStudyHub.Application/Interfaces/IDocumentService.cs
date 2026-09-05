using PRN212.AIStudyHub.Application.DTOs.Document;

namespace PRN212.AIStudyHub.Application.Interfaces;

public interface IDocumentService
{
  Task<DocumentResponseDto> UploadDocumentAsync(UploadDocumentCommand request, Guid userId, CancellationToken cancellationToken = default);
}
