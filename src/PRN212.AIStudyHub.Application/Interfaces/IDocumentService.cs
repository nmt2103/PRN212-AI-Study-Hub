using PRN212.AIStudyHub.Application.DTOs.Document;

namespace PRN212.AIStudyHub.Application.Interfaces;

public interface IDocumentService
{
  Task<DocumentResponseDto> UploadDocumentAsync(UploadDocumentCommand request, Guid userId, CancellationToken cancellationToken = default);

	Task<List<DocumentItemDto>> GetDocumentAsync(Guid userId, Guid? subjectId = null, CancellationToken cancellationToken = default);

	Task<DocumentResponseDto> GetDocumentDetailsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

	Task<bool> DeleteDocumentAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

	Task<DocumentResponseDto> UpdateDocumentAsync(Guid id, Guid userId, UpdateDocumentRequest request, CancellationToken cancellationToken = default);
}
