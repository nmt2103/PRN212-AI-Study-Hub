namespace PRN212.AIStudyHub.Domain.Entities;

public class RefreshToken
{
  public Guid Id { get; private set; }
  public Guid UserId { get; set; }
  public string Token { get; set; } = string.Empty;
  public DateTime ExpiresAt { get; set; }
  public bool IsRevoked { get; set; } = false;
  public DateTime CreatedAt { get; private set; }

  public virtual AppUser User { get; set; } = null!;

  public RefreshToken()
  {
    Id = Guid.CreateVersion7();
    CreatedAt = DateTime.UtcNow;
  }
}