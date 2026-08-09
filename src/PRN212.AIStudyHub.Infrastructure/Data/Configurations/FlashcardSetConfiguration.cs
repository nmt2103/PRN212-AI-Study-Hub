using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN212.AIStudyHub.Domain.Entities;

namespace PRN212.AIStudyHub.Infrastructure.Data.Configurations;

public class FlashcardSetConfiguration : IEntityTypeConfiguration<FlashcardSet>
{
  public void Configure(EntityTypeBuilder<FlashcardSet> builder)
  {
    builder.ToTable("FlashcardSet");
    builder.HasKey(fs => fs.Id);

    builder.HasOne(fs => fs.User)
        .WithMany(u => u.FlashcardSets)
        .HasForeignKey(fs => fs.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(fs => fs.Document)
        .WithMany(d => d.FlashcardSets)
        .HasForeignKey(fs => fs.DocumentId)
        .OnDelete(DeleteBehavior.Restrict);
  }
}