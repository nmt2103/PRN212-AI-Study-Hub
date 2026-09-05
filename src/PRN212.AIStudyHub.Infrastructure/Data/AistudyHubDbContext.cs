using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PRN212.AIStudyHub.Domain.Entities;

namespace PRN212.AIStudyHub.Infrastructure.Data;

public partial class AistudyHubDbContext : DbContext
{
  public AistudyHubDbContext(DbContextOptions<AistudyHubDbContext> options)
	  : base(options)
  {
  }

  public virtual DbSet<AppUser> AppUser { get; set; }

  public virtual DbSet<ChatMessage> ChatMessage { get; set; }

  public virtual DbSet<ChatSession> ChatSession { get; set; }

  public virtual DbSet<ChatSessionDocument> ChatSessionDocument { get; set; }

  public virtual DbSet<Document> Document { get; set; }

  public virtual DbSet<DocumentSummary> DocumentSummary { get; set; }

  public virtual DbSet<FlashcardItem> FlashcardItem { get; set; }

  public virtual DbSet<FlashcardSet> FlashcardSet { get; set; }

  public virtual DbSet<RefreshToken> RefreshToken { get; set; }

  public virtual DbSet<Subject> Subject { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
	modelBuilder.Entity<AppUser>(entity =>
	{
	  entity.HasKey(e => e.Id).HasName("PR_AppUser");

	  entity.HasIndex(e => e.Email, "UQ__AppUser__A9D10534142CC53F").IsUnique();

	  entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
	  entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
	  entity.Property(e => e.Email)
			  .HasMaxLength(255)
			  .IsUnicode(false);
	  entity.Property(e => e.FirstName).HasMaxLength(100);
	  entity.Property(e => e.IsActive).HasDefaultValue(true);
	  entity.Property(e => e.LastName).HasMaxLength(100);
	  entity.Property(e => e.PasswordHash)
			  .HasMaxLength(255)
			  .IsUnicode(false);
	  entity.Property(e => e.Role)
			  .HasMaxLength(50)
			  .IsUnicode(false)
			  .HasDefaultValue("Student");
	});

	modelBuilder.Entity<ChatMessage>(entity =>
	{
	  entity.HasIndex(e => e.SessionId, "IX_ChatMessage_SessionId");

	  entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
	  entity.Property(e => e.Sender)
			  .HasMaxLength(20)
			  .IsUnicode(false);
	  entity.Property(e => e.SentAt).HasDefaultValueSql("(sysutcdatetime())");

	  entity.HasOne(d => d.Session).WithMany(p => p.ChatMessage)
			  .HasForeignKey(d => d.SessionId)
			  .HasConstraintName("FK_ChatMessage_ChatSession");
	});

	modelBuilder.Entity<ChatSession>(entity =>
	{
	  entity.HasIndex(e => e.UserId, "IX_ChatSession_UserId");

	  entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
	  entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
	  entity.Property(e => e.Title).HasMaxLength(255);

	  entity.HasOne(d => d.User).WithMany(p => p.ChatSession)
			  .HasForeignKey(d => d.UserId)
			  .HasConstraintName("FK_ChatSession_AppUser");
	});

	modelBuilder.Entity<ChatSessionDocument>(entity =>
	{
	  entity.HasKey(e => new { e.SessionId, e.DocumentId });

	  entity.HasIndex(e => e.DocumentId, "IX_ChatSessionDocument_DocumentId");

	  entity.Property(e => e.AttachedAt).HasDefaultValueSql("(sysutcdatetime())");

	  entity.HasOne(d => d.Document).WithMany(p => p.ChatSessionDocument)
			  .HasForeignKey(d => d.DocumentId)
			  .OnDelete(DeleteBehavior.ClientSetNull)
			  .HasConstraintName("FK_CSD_Document");

	  entity.HasOne(d => d.Session).WithMany(p => p.ChatSessionDocument)
			  .HasForeignKey(d => d.SessionId)
			  .HasConstraintName("FK_CSD_ChatSession");
	});

	modelBuilder.Entity<Document>(entity =>
	{
	  entity.HasIndex(e => new { e.IsDeleted, e.SubjectId }, "IX_Document_IsDeleted_SubjectId");

	  entity.HasIndex(e => e.SubjectId, "IX_Document_SubjectId");

	  entity.HasIndex(e => e.UserId, "IX_Document_UserId");

	  entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
	  entity.Property(e => e.CloudPublicId).HasMaxLength(500);
	  entity.Property(e => e.ContentType)
			  .HasMaxLength(100)
			  .IsUnicode(false);
	  entity.Property(e => e.FileExtension)
			  .HasMaxLength(10)
			  .IsUnicode(false);
	  entity.Property(e => e.FileName).HasMaxLength(255);
	  entity.Property(e => e.ProcessingStatus)
			  .HasMaxLength(20)
			  .IsUnicode(false)
			  .HasDefaultValue("Pending");
	  entity.Property(e => e.StoragePath).HasMaxLength(2048);
	  entity.Property(e => e.Title).HasMaxLength(255);
	  entity.Property(e => e.UploadedAt).HasDefaultValueSql("(sysutcdatetime())");

	  entity.HasOne(d => d.Subject).WithMany(p => p.Document)
			  .HasForeignKey(d => d.SubjectId)
			  .HasConstraintName("FK_Document_Subject");

	  entity.HasOne(d => d.User).WithMany(p => p.Document)
			  .HasForeignKey(d => d.UserId)
			  .HasConstraintName("FK_Document_AppUser");
	});

	modelBuilder.Entity<DocumentSummary>(entity =>
	{
	  entity.HasIndex(e => e.DocumentId, "IX_DocumentSummary_DocumentId");

	  entity.HasIndex(e => e.DocumentId, "UQ__Document__1ABEEF0E36270CD4").IsUnique();

	  entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
	  entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

	  entity.HasOne(d => d.Document).WithOne(p => p.DocumentSummary)
			  .HasForeignKey<DocumentSummary>(d => d.DocumentId)
			  .HasConstraintName("FK_DocumentSummary_Document");
	});

	modelBuilder.Entity<FlashcardItem>(entity =>
	{
	  entity.HasIndex(e => e.SetId, "IX_FlashcardItem_SetId");

	  entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
	  entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

	  entity.HasOne(d => d.Set).WithMany(p => p.FlashcardItem)
			  .HasForeignKey(d => d.SetId)
			  .HasConstraintName("FK_FlashcardItem_FlashcardSet");
	});

	modelBuilder.Entity<FlashcardSet>(entity =>
	{
	  entity.HasIndex(e => e.DocumentId, "IX_FlashcardSet_DocumentId");

	  entity.HasIndex(e => e.UserId, "IX_FlashcardSet_UserId");

	  entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
	  entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
	  entity.Property(e => e.Description).HasMaxLength(500);
	  entity.Property(e => e.Title).HasMaxLength(255);

	  entity.HasOne(d => d.Document).WithMany(p => p.FlashcardSet)
			  .HasForeignKey(d => d.DocumentId)
			  .HasConstraintName("FK_FlashcardSet_Document");

	  entity.HasOne(d => d.User).WithMany(p => p.FlashcardSet)
			  .HasForeignKey(d => d.UserId)
			  .HasConstraintName("FK_FlashcardSet_AppUser");
	});

	modelBuilder.Entity<RefreshToken>(entity =>
	{
	  entity.HasIndex(e => e.UserId, "IX_RefreshToken_UserId");

	  entity.HasIndex(e => e.Token, "UQ__RefreshT__1EB4F817B4D062CE").IsUnique();

	  entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
	  entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
	  entity.Property(e => e.Token)
			  .HasMaxLength(500)
			  .IsUnicode(false);

	  entity.HasOne(d => d.User).WithMany(p => p.RefreshToken)
			  .HasForeignKey(d => d.UserId)
			  .HasConstraintName("FK_RefreshToken_AppUser");
	});

	modelBuilder.Entity<Subject>(entity =>
	{
	  entity.HasIndex(e => e.Name, "UQ__Subject__737584F64DCE1AC5").IsUnique();

	  entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
	  entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
	  entity.Property(e => e.Description).HasMaxLength(500);
	  entity.Property(e => e.Name).HasMaxLength(100);
	});

	OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
