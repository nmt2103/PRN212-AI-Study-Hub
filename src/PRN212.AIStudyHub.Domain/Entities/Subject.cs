namespace PRN212.AIStudyHub.Domain.Entities;

public class Subject
{
  public Guid Id { get; private set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime CreatedAt { get; private set; }

  public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

  public Subject()
  {
    Id = Guid.CreateVersion7();
    CreatedAt = DateTime.UtcNow;
  }
}