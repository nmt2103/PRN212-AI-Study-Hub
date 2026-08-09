namespace PRN212.AIStudyHub.Domain.Entities;

public class ChatMessage
{
  public Guid Id { get; private set; }
  public Guid SessionId { get; set; }
  public string Sender { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public DateTime SentAt { get; private set; }

  public virtual ChatSession ChatSession { get; set; } = null!;

  public ChatMessage()
  {
    Id = Guid.CreateVersion7();
    SentAt = DateTime.UtcNow;
  }
}