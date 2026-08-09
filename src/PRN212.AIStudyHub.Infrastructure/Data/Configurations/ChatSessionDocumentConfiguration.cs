using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN212.AIStudyHub.Domain.Entities;

namespace PRN212.AIStudyHub.Infrastructure.Data.Configurations;

public class ChatSessionDocumentConfiguration : IEntityTypeConfiguration<ChatSessionDocument>
{
  public void Configure(EntityTypeBuilder<ChatSessionDocument> builder)
  {
    builder.ToTable("ChatSessionDocument");
    builder.HasKey(csd => new { csd.SessionId, csd.DocumentId });

    builder.HasOne(csd => csd.ChatSession)
        .WithMany(cs => cs.SessionDocuments)
        .HasForeignKey(csd => csd.SessionId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(csd => csd.Document)
        .WithMany(d => d.SessionDocuments)
        .HasForeignKey(csd => csd.DocumentId)
        .OnDelete(DeleteBehavior.Restrict);
  }
}