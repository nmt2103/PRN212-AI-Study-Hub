namespace PRN212.AIStudyHub.Domain.Entities;

public class DocumentSummary
{
  public Guid Id { get; private set; }
  public Guid DocumentId { get; set; }
  public string SummaryContent { get; set; } = string.Empty;
  public string? KeyTakeaways { get; set; }
  public DateTime CreatedAt { get; private set; }
  public DateTime? UpdatedAt { get; set; }

  public virtual Document Document { get; set; } = null!;

  public DocumentSummary()
  {
    Id = Guid.CreateVersion7();
    CreatedAt = DateTime.UtcNow;
  }
}