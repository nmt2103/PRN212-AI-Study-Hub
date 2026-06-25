namespace Prn212.AIStudyHub.WPF.Entities;

public partial class Document
{
  public int Id { get; set; }

  public int UserId { get; set; }

  public int SubjectId { get; set; }

  public string Title { get; set; } = null!;

  public string FileName { get; set; } = null!;

  public string StoragePath { get; set; } = null!;

  public long FileSize { get; set; }

  public string FileExtension { get; set; } = null!;

  public string ContentType { get; set; } = null!;

  public DateTime UploadedAt { get; set; }

  public virtual Subject Subject { get; set; } = null!;

  public virtual AppUser User { get; set; } = null!;
}
