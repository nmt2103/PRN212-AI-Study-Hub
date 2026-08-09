namespace PRN212.AIStudyHub.Domain.Entities;

public class Document
{
  public Guid Id { get; private set; }
  public Guid UserId { get; set; }
  public Guid SubjectId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string FileName { get; set; } = string.Empty;
  public string StoragePath { get; set; } = string.Empty;
  public long FileSize { get; set; }
  public string FileExtension { get; set; } = string.Empty;
  public string ContentType { get; set; } = string.Empty;
  public DateTime UploadedAt { get; private set; }
  public bool IsCloudStored { get; set; } = false;
  public string? CloudPublicId { get; set; }
  public bool IsPublic { get; set; } = false;
  public string ProcessingStatus { get; set; } = "Pending";
  public bool IsDeleted { get; set; } = false;
  public DateTime? DeletedAt { get; set; }

  public virtual AppUser User { get; set; } = null!;
  public virtual Subject Subject { get; set; } = null!;
  public virtual DocumentSummary? Summary { get; set; }
  public virtual ICollection<ChatSessionDocument> SessionDocuments { get; set; } = new List<ChatSessionDocument>();
  public virtual ICollection<FlashcardSet> FlashcardSets { get; set; } = new List<FlashcardSet>();

  public Document()
  {
    Id = Guid.CreateVersion7();
    UploadedAt = DateTime.UtcNow;
  }
}