using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN212.AIStudyHub.Domain.Entities;

namespace PRN212.AIStudyHub.Infrastructure.Data.Configurations;

public class DocumentSummaryConfiguration : IEntityTypeConfiguration<DocumentSummary>
{
  public void Configure(EntityTypeBuilder<DocumentSummary> builder)
  {
    builder.ToTable("DocumentSummary");
    builder.HasKey(ds => ds.Id);

    builder.HasIndex(ds => ds.DocumentId).IsUnique();

    builder.HasOne(ds => ds.Document)
        .WithOne(d => d.Summary)
        .HasForeignKey<DocumentSummary>(ds => ds.DocumentId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}