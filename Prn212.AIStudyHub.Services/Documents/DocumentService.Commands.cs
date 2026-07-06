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

    public async Task DownloadAsync(int documentId, string destinationFilePath, IProgress<double>? progress = null)
    {
        using var context = new AistudyHubDbContext();
        var doc = await context.Documents.FindAsync(documentId);
        if (doc == null)
            throw new Exception("Tài liệu không tồn tại trên hệ thống");

        string sourcePath = Path.Combine(AppContext.BaseDirectory, doc.StoragePath);
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Không tìm thấy file gốc trên server");

        const int bufferSize = 81920;
        byte[] buffer = new byte[bufferSize];

        using var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
        using var destStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write);

        long totalBytes = sourceStream.Length;
        long bytesRead = 0;
        int read;

        while ((read = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await destStream.WriteAsync(buffer, 0, read);
            bytesRead += read;

            if (progress != null && totalBytes > 0)
            {
                double percent = (double)bytesRead / totalBytes * 100;
                progress.Report(percent);
            }
        }
    }

    public async Task UpdateMetadataAsync(int documentId, string title, int subjectId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Tiêu đề tài liệu không được để trống.");

        using var context = new AistudyHubDbContext();

        var doc = await context.Documents.FindAsync(documentId);
        if (doc == null)
            throw new KeyNotFoundException("Không tìm thấy tài liệu cần chỉnh sửa trên hệ thống.");

        doc.Title = title.Trim();
        doc.SubjectId = subjectId;

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int documentId)
    {
        using var context = new AistudyHubDbContext();
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var doc = await context.Documents.FindAsync(documentId);
            if (doc == null)
                throw new KeyNotFoundException("Không tìm thấy tài liệu cần xóa trên hệ thống.");

            string fullPath = Path.Combine(AppContext.BaseDirectory, doc.StoragePath);

            // Xóa trong database trước
            context.Documents.Remove(doc);
            await context.SaveChangesAsync();

            // Xóa file vật lý
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // Commit transaction
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
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
