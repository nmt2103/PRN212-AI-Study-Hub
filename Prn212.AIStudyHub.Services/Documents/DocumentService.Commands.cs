using Prn212.AIStudyHub.DataAccess;

namespace Prn212.AIStudyHub.Services.Documents;

/// <summary>
/// Phần GHI: upload / sửa / xóa. NGƯỜI DOC 1 làm ở file này.
/// (DocumentService là 1 class được chia làm 2 file - xem thêm DocumentService.Queries.cs)
/// </summary>
public partial class DocumentService
{
  private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50 MB
  private static readonly string[] AllowedExtensions =
      { ".pdf", ".docx", ".xlsx", ".pptx", ".txt", ".md" };

  public async Task<Document> UploadAsync(int userId, int subjectId, string title, string sourceFilePath)
  {
    if (string.IsNullOrWhiteSpace(sourceFilePath) || !File.Exists(sourceFilePath))
      throw new FileNotFoundException("Tệp nguồn không tồn tại.");

    var fileInfo = new FileInfo(sourceFilePath);
    if (fileInfo.Length > MaxFileSizeBytes)
      throw new InvalidOperationException("Dung lượng tệp vượt quá 50MB.");

    string extension = Path.GetExtension(sourceFilePath).ToLowerInvariant();
    if (!AllowedExtensions.Contains(extension))
      throw new InvalidOperationException("Định dạng tệp không được hỗ trợ.");

    string uploadDir = Path.Combine(AppContext.BaseDirectory, "uploads");
    Directory.CreateDirectory(uploadDir);

    string uniqueFileName = $"{Guid.NewGuid()}_{fileInfo.Name}";
    string destinationPath = Path.Combine(uploadDir, uniqueFileName);

    using (var sourceStream = File.OpenRead(sourceFilePath))
    using (var destStream = File.Create(destinationPath))
    {
      await sourceStream.CopyToAsync(destStream);
    }

    var document = new Document
    {
      UserId = userId,
      SubjectId = subjectId,
      Title = title.Trim(),
      FileName = fileInfo.Name,
      StoragePath = Path.Combine("uploads", uniqueFileName),
      FileSize = fileInfo.Length,
      FileExtension = extension,
      ContentType = GetContentType(extension),
      UploadedAt = DateTime.UtcNow
    };

    try
    {
      using var context = new AistudyHubDbContext();
      context.Documents.Add(document);
      await context.SaveChangesAsync();
      return document;
    }
    catch
    {
      if (File.Exists(destinationPath))
        File.Delete(destinationPath);
      throw;
    }
  }

  public async Task UpdateMetadataAsync(int documentId, string title, int subjectId)
  {
    // TODO (DOC 1): lấy document theo id, cập nhật Title/SubjectId, SaveChanges.
    await Task.CompletedTask;
    throw new NotImplementedException("Cài đặt UpdateMetadata tại đây.");
  }

  public async Task DeleteAsync(int documentId)
  {
    // TODO (DOC 1): lấy document, xóa file vật lý + xóa bản ghi, SaveChanges.
    await Task.CompletedTask;
    throw new NotImplementedException("Cài đặt Delete tại đây.");
  }

  private static string GetContentType(string extension) => extension.ToLowerInvariant() switch
  {
    ".pdf" => "application/pdf",
    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
    ".txt" => "text/plain",
    ".md" => "text/markdown",
    _ => "application/octet-stream"
  };
}
