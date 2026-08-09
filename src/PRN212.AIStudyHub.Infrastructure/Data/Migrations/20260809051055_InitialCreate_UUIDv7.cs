using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN212.AIStudyHub.Infrastructure.Data.Migrations
{
  /// <inheritdoc />
  public partial class InitialCreate_UUIDv7 : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "AppUsers",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
            FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_AppUsers", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Subjects",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Subjects", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "ChatSessions",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ChatSessions", x => x.Id);
            table.ForeignKey(
                      name: "FK_ChatSessions_AppUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AppUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "RefreshTokens",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            IsRevoked = table.Column<bool>(type: "bit", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_RefreshTokens", x => x.Id);
            table.ForeignKey(
                      name: "FK_RefreshTokens_AppUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AppUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Documents",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            StoragePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
            FileSize = table.Column<long>(type: "bigint", nullable: false),
            FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
            ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            IsCloudStored = table.Column<bool>(type: "bit", nullable: false),
            CloudPublicId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsPublic = table.Column<bool>(type: "bit", nullable: false),
            ProcessingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Documents", x => x.Id);
            table.ForeignKey(
                      name: "FK_Documents_AppUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AppUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Documents_Subjects_SubjectId",
                      column: x => x.SubjectId,
                      principalTable: "Subjects",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "ChatMessages",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Sender = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
            SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            ChatSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ChatMessages", x => x.Id);
            table.ForeignKey(
                      name: "FK_ChatMessages_ChatSessions_ChatSessionId",
                      column: x => x.ChatSessionId,
                      principalTable: "ChatSessions",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "ChatSessionDocument",
          columns: table => new
          {
            SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            AttachedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_ChatSessionDocument", x => new { x.SessionId, x.DocumentId });
            table.ForeignKey(
                      name: "FK_ChatSessionDocument_ChatSessions_SessionId",
                      column: x => x.SessionId,
                      principalTable: "ChatSessions",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_ChatSessionDocument_Documents_DocumentId",
                      column: x => x.DocumentId,
                      principalTable: "Documents",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "DocumentSummary",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SummaryContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
            KeyTakeaways = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_DocumentSummary", x => x.Id);
            table.ForeignKey(
                      name: "FK_DocumentSummary_Documents_DocumentId",
                      column: x => x.DocumentId,
                      principalTable: "Documents",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "FlashcardSet",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_FlashcardSet", x => x.Id);
            table.ForeignKey(
                      name: "FK_FlashcardSet_AppUsers_UserId",
                      column: x => x.UserId,
                      principalTable: "AppUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_FlashcardSet_Documents_DocumentId",
                      column: x => x.DocumentId,
                      principalTable: "Documents",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "FlashcardItems",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
            IsMastered = table.Column<bool>(type: "bit", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            FlashcardSetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_FlashcardItems", x => x.Id);
            table.ForeignKey(
                      name: "FK_FlashcardItems_FlashcardSet_FlashcardSetId",
                      column: x => x.FlashcardSetId,
                      principalTable: "FlashcardSet",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_ChatMessages_ChatSessionId",
          table: "ChatMessages",
          column: "ChatSessionId");

      migrationBuilder.CreateIndex(
          name: "IX_ChatSessionDocument_DocumentId",
          table: "ChatSessionDocument",
          column: "DocumentId");

      migrationBuilder.CreateIndex(
          name: "IX_ChatSessions_UserId",
          table: "ChatSessions",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_Documents_SubjectId",
          table: "Documents",
          column: "SubjectId");

      migrationBuilder.CreateIndex(
          name: "IX_Documents_UserId",
          table: "Documents",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_DocumentSummary_DocumentId",
          table: "DocumentSummary",
          column: "DocumentId",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_FlashcardItems_FlashcardSetId",
          table: "FlashcardItems",
          column: "FlashcardSetId");

      migrationBuilder.CreateIndex(
          name: "IX_FlashcardSet_DocumentId",
          table: "FlashcardSet",
          column: "DocumentId");

      migrationBuilder.CreateIndex(
          name: "IX_FlashcardSet_UserId",
          table: "FlashcardSet",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_RefreshTokens_UserId",
          table: "RefreshTokens",
          column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "ChatMessages");

      migrationBuilder.DropTable(
          name: "ChatSessionDocument");

      migrationBuilder.DropTable(
          name: "DocumentSummary");

      migrationBuilder.DropTable(
          name: "FlashcardItems");

      migrationBuilder.DropTable(
          name: "RefreshTokens");

      migrationBuilder.DropTable(
          name: "ChatSessions");

      migrationBuilder.DropTable(
          name: "FlashcardSet");

      migrationBuilder.DropTable(
          name: "Documents");

      migrationBuilder.DropTable(
          name: "AppUsers");

      migrationBuilder.DropTable(
          name: "Subjects");
    }
  }
}
