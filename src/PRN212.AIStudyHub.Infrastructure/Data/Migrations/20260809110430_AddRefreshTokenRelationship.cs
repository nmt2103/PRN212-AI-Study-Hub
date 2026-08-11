using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN212.AIStudyHub.Infrastructure.Data.Migrations
{
  /// <inheritdoc />
  public partial class AddRefreshTokenRelationship : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_RefreshTokens_AppUsers_UserId",
          table: "RefreshTokens");

      migrationBuilder.DropPrimaryKey(
          name: "PK_RefreshTokens",
          table: "RefreshTokens");

      migrationBuilder.RenameTable(
          name: "RefreshTokens",
          newName: "RefreshToken");

      migrationBuilder.RenameIndex(
          name: "IX_RefreshTokens_UserId",
          table: "RefreshToken",
          newName: "IX_RefreshToken_UserId");

      migrationBuilder.AlterColumn<string>(
          name: "Token",
          table: "RefreshToken",
          type: "nvarchar(256)",
          maxLength: 256,
          nullable: false,
          oldClrType: typeof(string),
          oldType: "nvarchar(max)");

      migrationBuilder.AddColumn<Guid>(
          name: "AppUserId",
          table: "RefreshToken",
          type: "uniqueidentifier",
          nullable: true);

      migrationBuilder.AddPrimaryKey(
          name: "PK_RefreshToken",
          table: "RefreshToken",
          column: "Id");

      migrationBuilder.CreateIndex(
          name: "IX_RefreshToken_AppUserId",
          table: "RefreshToken",
          column: "AppUserId");

      migrationBuilder.AddForeignKey(
          name: "FK_RefreshToken_AppUsers_AppUserId",
          table: "RefreshToken",
          column: "AppUserId",
          principalTable: "AppUsers",
          principalColumn: "Id");

      migrationBuilder.AddForeignKey(
          name: "FK_RefreshToken_AppUsers_UserId",
          table: "RefreshToken",
          column: "UserId",
          principalTable: "AppUsers",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_RefreshToken_AppUsers_AppUserId",
          table: "RefreshToken");

      migrationBuilder.DropForeignKey(
          name: "FK_RefreshToken_AppUsers_UserId",
          table: "RefreshToken");

      migrationBuilder.DropPrimaryKey(
          name: "PK_RefreshToken",
          table: "RefreshToken");

      migrationBuilder.DropIndex(
          name: "IX_RefreshToken_AppUserId",
          table: "RefreshToken");

      migrationBuilder.DropColumn(
          name: "AppUserId",
          table: "RefreshToken");

      migrationBuilder.RenameTable(
          name: "RefreshToken",
          newName: "RefreshTokens");

      migrationBuilder.RenameIndex(
          name: "IX_RefreshToken_UserId",
          table: "RefreshTokens",
          newName: "IX_RefreshTokens_UserId");

      migrationBuilder.AlterColumn<string>(
          name: "Token",
          table: "RefreshTokens",
          type: "nvarchar(max)",
          nullable: false,
          oldClrType: typeof(string),
          oldType: "nvarchar(256)",
          oldMaxLength: 256);

      migrationBuilder.AddPrimaryKey(
          name: "PK_RefreshTokens",
          table: "RefreshTokens",
          column: "Id");

      migrationBuilder.AddForeignKey(
          name: "FK_RefreshTokens_AppUsers_UserId",
          table: "RefreshTokens",
          column: "UserId",
          principalTable: "AppUsers",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);
    }
  }
}
