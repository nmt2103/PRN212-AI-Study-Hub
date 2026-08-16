namespace PRN212.AIStudyHub.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN212.AIStudyHub.Domain.Entities;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
  public void Configure(EntityTypeBuilder<Document> builder)
  {
    builder.ToTable("Document");

    builder.HasKey(d => d.Id);

    builder.Property(d => d.Title).IsRequired().HasMaxLength(500);
    builder.Property(d => d.FileName).IsRequired().HasMaxLength(255);
    builder.Property(d => d.StoragePath).HasMaxLength(1000);
    builder.Property(d => d.CloudPublicId).HasMaxLength(255);

    builder.HasOne(d => d.User)
           .WithMany(u => u.Documents)
           .HasForeignKey(d => d.UserId)
           .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(d => d.Subject)
           .WithMany(s => s.Documents)
           .HasForeignKey(d => d.SubjectId)
           .OnDelete(DeleteBehavior.Restrict);

    builder.HasQueryFilter(d => !d.IsDeleted);

    builder.HasIndex(d => d.SubjectId);
    builder.HasIndex(d => d.UserId);
  }
}
