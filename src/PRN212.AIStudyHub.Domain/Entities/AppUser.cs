namespace PRN212.AIStudyHub.Domain.Entities;

public class AppUser
{
  public Guid Id { get; private set; }
  public string Email { get; set; } = string.Empty;
  public string PasswordHash { get; set; } = string.Empty;
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Role { get; set; } = "Student";
  public bool IsActive { get; set; } = true;
  public DateTime CreatedAt { get; private set; }
  public DateTime? UpdatedAt { get; set; }

  public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
  public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
  public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
  public virtual ICollection<FlashcardSet> FlashcardSets { get; set; } = new List<FlashcardSet>();

  public AppUser()
  {
    Id = Guid.CreateVersion7();
    CreatedAt = DateTime.UtcNow;
  }
}