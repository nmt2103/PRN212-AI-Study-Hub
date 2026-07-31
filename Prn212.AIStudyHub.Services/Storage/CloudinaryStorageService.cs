using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace Prn212.AIStudyHub.Services.Storage;

/// <summary>
/// Dịch vụ upload / xóa tệp tin trên Cloudinary (lưu trữ đám mây).
/// Đọc cấu hình từ appsettings.json (section "Cloudinary"), giống cách AistudyHubDbContext đọc ConnectionStrings.
/// </summary>
public class CloudinaryStorageService
{
  private static readonly Lazy<IConfiguration> _config = new(() =>
      new ConfigurationBuilder()
          .SetBasePath(AppContext.BaseDirectory)
          .AddJsonFile("appsettings.json", false, false)
          .Build());

  private static readonly Lazy<Cloudinary> _cloudinary = new(() =>
  {
    var config = _config.Value;

    string cloudName = config["Cloudinary:CloudName"]
          ?? throw new InvalidOperationException("Thiếu cấu hình 'Cloudinary:CloudName' trong appsettings.json.");
    string apiKey = config["Cloudinary:ApiKey"]
          ?? throw new InvalidOperationException("Thiếu cấu hình 'Cloudinary:ApiKey' trong appsettings.json.");
    string apiSecret = config["Cloudinary:ApiSecret"]
          ?? throw new InvalidOperationException("Thiếu cấu hình 'Cloudinary:ApiSecret' trong appsettings.json.");

    var account = new Account(cloudName, apiKey, apiSecret);
    return new Cloudinary(account) { Api = { Secure = true } };
  });

  private static string Folder => _config.Value["Cloudinary:Folder"] ?? "ai-study-hub/documents";

  /// <summary>
  /// Upload một tệp (pdf/docx/xlsx/pptx/txt/md...) lên Cloudinary dưới dạng "raw".
  /// Trả về URL công khai (secure) và PublicId để lưu vào DB.
  /// </summary>
  public async Task<(string SecureUrl, string PublicId)> UploadFileAsync(string filePath, string fileNameOnCloud)
  {
    if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
      throw new FileNotFoundException("Không tìm thấy tệp để upload lên Cloudinary.", filePath);

    using var stream = File.OpenRead(filePath);

    var uploadParams = new RawUploadParams
    {
      File = new FileDescription(fileNameOnCloud, stream),
      Folder = Folder,
      UseFilename = true,
      UniqueFilename = true,
      Overwrite = false
    };

    var result = await _cloudinary.Value.UploadAsync(uploadParams);

    if (result.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrEmpty(result.SecureUrl?.ToString()))
      throw new InvalidOperationException($"Upload Cloudinary thất bại: {result.Error?.Message ?? "Lỗi không xác định"}");

    return (result.SecureUrl!.ToString(), result.PublicId);
  }

  /// <summary>
  /// Xóa một tệp raw đã upload trên Cloudinary theo PublicId.
  /// </summary>
  public async Task DeleteFileAsync(string? publicId)
  {
    if (string.IsNullOrWhiteSpace(publicId))
      return;

    var deletionParams = new DeletionParams(publicId) { ResourceType = ResourceType.Raw };
    await _cloudinary.Value.DestroyAsync(deletionParams);
  }
}