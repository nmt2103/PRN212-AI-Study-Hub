namespace PRN212.AIStudyHub.Domain.Entities;

public class FlashcardItem
{
  public Guid Id { get; private set; }
  public Guid SetId { get; set; }
  public string Question { get; set; } = string.Empty;
  public string Answer { get; set; } = string.Empty;
  public bool IsMastered { get; set; } = false;
  public DateTime CreatedAt { get; private set; }

  public virtual FlashcardSet FlashcardSet { get; set; } = null!;

  public FlashcardItem()
  {
    Id = Guid.CreateVersion7();
    CreatedAt = DateTime.UtcNow;
  }
}