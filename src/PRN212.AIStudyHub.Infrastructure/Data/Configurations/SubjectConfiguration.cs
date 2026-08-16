namespace PRN212.AIStudyHub.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN212.AIStudyHub.Domain.Entities;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
  public void Configure(EntityTypeBuilder<Subject> builder)
  {
    builder.ToTable("Subject");

    builder.HasKey(s => s.Id);
    
    builder.Property(s => s.Name).IsRequired().HasMaxLength(255);
    builder.Property(s => s.Description).HasMaxLength(1000);
  }
}
