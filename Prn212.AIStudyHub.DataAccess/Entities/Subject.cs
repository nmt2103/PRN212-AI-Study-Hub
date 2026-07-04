namespace Prn212.AIStudyHub.DataAccess;

public partial class Subject
{
  public int Id { get; set; }
  public string Name { get; set; } = null!;
  public string? Description { get; set; }
  public DateTime CreatedAt { get; set; }
  public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}
