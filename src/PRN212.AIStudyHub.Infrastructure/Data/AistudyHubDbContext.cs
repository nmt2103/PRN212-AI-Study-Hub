using Microsoft.EntityFrameworkCore;
using PRN212.AIStudyHub.Application.Interfaces;
using PRN212.AIStudyHub.Domain.Entities;

namespace PRN212.AIStudyHub.Infrastructure.Data;

public class AistudyHubDbContext : DbContext, IAppDbContext
{
  public AistudyHubDbContext(DbContextOptions<AistudyHubDbContext> options) : base(options) { }

  public DbSet<AppUser> AppUsers => Set<AppUser>();
  public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
  public DbSet<Subject> Subjects => Set<Subject>();
  public DbSet<Document> Documents => Set<Document>();
  public DbSet<DocumentSummary> DocumentSummaries => Set<DocumentSummary>();
  public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
  public DbSet<ChatSessionDocument> ChatSessionDocuments => Set<ChatSessionDocument>();
  public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
  public DbSet<FlashcardSet> FlashcardSets => Set<FlashcardSet>();
  public DbSet<FlashcardItem> FlashcardItems => Set<FlashcardItem>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Map all DbSets to singular table names (matching Entity names)
    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    {
      entity.SetTableName(entity.ClrType.Name);
    }

    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AistudyHubDbContext).Assembly);
  }
}