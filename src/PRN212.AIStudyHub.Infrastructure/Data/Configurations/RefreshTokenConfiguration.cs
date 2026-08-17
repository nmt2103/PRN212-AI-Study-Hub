namespace PRN212.AIStudyHub.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN212.AIStudyHub.Domain.Entities;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
  public void Configure(EntityTypeBuilder<RefreshToken> builder)
  {
    builder.ToTable("RefreshToken");
    builder.HasKey(r => r.Id);
    builder.Property(r => r.Token).IsRequired().HasMaxLength(256);

    builder.HasOne(r => r.User)
           .WithMany(u => u.RefreshTokens)
           .HasForeignKey(r => r.UserId)
           .OnDelete(DeleteBehavior.Cascade);
  }
}
