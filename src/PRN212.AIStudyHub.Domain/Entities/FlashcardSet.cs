namespace PRN212.AIStudyHub.Domain.Entities;

public class FlashcardSet
{
  public Guid Id { get; private set; }
  public Guid UserId { get; set; }
  public Guid? DocumentId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime CreatedAt { get; private set; }

  public virtual AppUser User { get; set; } = null!;
  public virtual Document? Document { get; set; }
  public virtual ICollection<FlashcardItem> Items { get; set; } = new List<FlashcardItem>();

  public FlashcardSet()
  {
    Id = Guid.CreateVersion7();
    CreatedAt = DateTime.UtcNow;
  }
}