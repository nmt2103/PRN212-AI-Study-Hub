using PRN212.AIStudyHub.Application.DTOs.Document;

namespace PRN212.AIStudyHub.Application.Interfaces;

public interface IDocumentService
{
  Task<DocumentResponseDto> UploadDocument(UploadDocumentCommand request, Guid userId, CancellationToken cancellationToken = default);

  Task<List<DocumentItemDto>> GetMyDocuments(Guid userId, Guid? subjectId = null, CancellationToken cancellationToken = default);

  Task<DocumentResponseDto> GetDocumentById(Guid id, Guid userId, CancellationToken cancellationToken = default);

  Task<bool> DeleteDocument(Guid id, Guid userId, CancellationToken cancellationToken = default);

  Task<DocumentResponseDto> UpdateDocument(Guid id, Guid userId, UpdateDocumentRequest request, CancellationToken cancellationToken = default);
}
