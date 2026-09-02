using PRN212.AIStudyHub.Application.DTOs.Cloud;

namespace PRN212.AIStudyHub.Application.Services.Cloud;

public interface ICloudStorageService
{
  Task<CloudUploadResult> UploadRawFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}
