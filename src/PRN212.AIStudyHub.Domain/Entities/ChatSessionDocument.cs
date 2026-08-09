namespace PRN212.AIStudyHub.Domain.Entities;

public class ChatSessionDocument
{
  public Guid SessionId { get; set; }
  public Guid DocumentId { get; set; }
  public DateTime AttachedAt { get; private set; }

  public virtual ChatSession ChatSession { get; set; } = null!;
  public virtual Document Document { get; set; } = null!;

  public ChatSessionDocument()
  {
    AttachedAt = DateTime.UtcNow;
  }
}