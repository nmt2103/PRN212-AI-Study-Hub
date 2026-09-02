using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using PRN212.AIStudyHub.Application.DTOs.Cloud;
using PRN212.AIStudyHub.Application.Services.Cloud;

namespace PRN212.AIStudyHub.Infrastructure.Cloud;

public class CloudinaryStorageService : ICloudStorageService
{
  private readonly Cloudinary _cloudinary;
  private readonly string _folder;

  public CloudinaryStorageService(IOptions<CloudinarySettings> config)
  {
    var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
    _cloudinary = new Cloudinary(account);
    _folder = config.Value.Folder;
  }

  public async Task<CloudUploadResult> UploadRawFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
  {
    var uploadParams = new RawUploadParams
    {
      File = new FileDescription(fileName, fileStream),
      Folder = _folder,
      UseFilename = true,
      UniqueFilename = true
    };

    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

    if (uploadResult.Error != null)
    {
      throw new Exception($"Cloudinary Upload Failed: \n{uploadResult.Error.Message}");
    }

    return new CloudUploadResult(PublicId: uploadResult.PublicId, Url: uploadResult.Url.ToString(), SecureUrl: uploadResult.SecureUrl.ToString(), Bytes: uploadResult.Bytes, Format: uploadResult.Format ?? "unknown");
  }
}