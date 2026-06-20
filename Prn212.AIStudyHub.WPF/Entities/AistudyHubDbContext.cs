using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Prn212.AIStudyHub.WPF.Entities;

public partial class AistudyHubDbContext : DbContext
{
  public AistudyHubDbContext()
  {
  }

  public AistudyHubDbContext(DbContextOptions<AistudyHubDbContext> options)
      : base(options)
  {
  }

  public virtual DbSet<AppUser> AppUsers { get; set; }

  public virtual DbSet<Document> Documents { get; set; }

  public virtual DbSet<Subject> Subjects { get; set; }

  private string GetConnectionString()
  {
    IConfiguration config = new ConfigurationBuilder()
         .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .Build();
    var strConn = config["ConnectionStrings:DefaultConnection"];

#pragma warning disable CS8603 // Possible null reference return.
    return strConn;
#pragma warning restore CS8603 // Possible null reference return.
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseSqlServer(GetConnectionString());
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<AppUser>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PK__AppUser__3214EC076FF22AB7");

      entity.ToTable("AppUser");

      entity.HasIndex(e => e.Email, "UQ__AppUser__A9D10534A016955B").IsUnique();

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

    modelBuilder.Entity<Document>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PK__Document__3214EC072FA3EAD7");

      entity.ToTable("Document");

      entity.HasIndex(e => e.SubjectId, "IX_Document_SubjectId");

      entity.HasIndex(e => e.UserId, "IX_Document_UserId");

      entity.Property(e => e.ContentType)
              .HasMaxLength(100)
              .IsUnicode(false);
      entity.Property(e => e.FileExtension)
              .HasMaxLength(10)
              .IsUnicode(false);
      entity.Property(e => e.FileName).HasMaxLength(255);
      entity.Property(e => e.StoragePath).HasMaxLength(2048);
      entity.Property(e => e.Title).HasMaxLength(255);
      entity.Property(e => e.UploadedAt).HasDefaultValueSql("(sysutcdatetime())");

      entity.HasOne(d => d.Subject).WithMany(p => p.Documents)
              .HasForeignKey(d => d.SubjectId)
              .HasConstraintName("FK_Document_Subject");

      entity.HasOne(d => d.User).WithMany(p => p.Documents)
              .HasForeignKey(d => d.UserId)
              .HasConstraintName("FK_Document_User");
    });

    modelBuilder.Entity<Subject>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PK__Subject__3214EC075109024D");

      entity.ToTable("Subject");

      entity.HasIndex(e => e.Name, "UQ__Subject__737584F63CA7C47F").IsUnique();

      entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
      entity.Property(e => e.Description).HasMaxLength(500);
      entity.Property(e => e.Name).HasMaxLength(100);
    });

    OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
