namespace PRN212.AIStudyHub.Domain.Entities;

public class ChatSession
{
  public Guid Id { get; private set; }
  public Guid UserId { get; set; }
  public string Title { get; set; } = string.Empty;
  public DateTime CreatedAt { get; private set; }
  public DateTime? UpdatedAt { get; set; }

  // Navigation Properties
  public virtual AppUser User { get; set; } = null!;
  public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
  public virtual ICollection<ChatSessionDocument> SessionDocuments { get; set; } = new List<ChatSessionDocument>();

  public ChatSession()
  {
    Id = Guid.CreateVersion7();
    CreatedAt = DateTime.UtcNow;
  }
}