using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using PRN212.AIStudyHub.Application.DTOs.Cloud;
using PRN212.AIStudyHub.Application.Exceptions;
using PRN212.AIStudyHub.Application.Interfaces.Cloud;

namespace PRN212.AIStudyHub.Infrastructure.Cloud;

public class CloudinaryStorageService : ICloudStorageService
{
  private readonly Cloudinary _cloudinary;
  private readonly string _folder;
  private readonly IHttpClientFactory _httpClientFactory;

  public CloudinaryStorageService(
		IOptions<CloudinarySettings> config,
		IHttpClientFactory httpClientFactory)
  {
	var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
	_cloudinary = new Cloudinary(account);
	_folder = config.Value.Folder;
	_httpClientFactory = httpClientFactory;
  }

  public async Task<CloudUploadResult> UploadRawFile(
	  Stream fileStream,
	  string fileName,
	  CancellationToken cancellationToken = default)
  {
	try
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
		throw new CloudStorageException($"Cloudinary Upload Failed: {uploadResult.Error.Message}");
	  }

	  return new CloudUploadResult(
	  PublicId: uploadResult.PublicId,
	  Url: uploadResult.Url.ToString(),
	  SecureUrl: uploadResult.SecureUrl.ToString(),
	  Bytes: uploadResult.Bytes,
	  Format: uploadResult.Format ?? "unknown"
	  );
	}
	catch (CloudStorageException)
	{
	  throw;
	}
	catch (Exception ex)
	{
	  throw new CloudStorageException($"Failed to connect to Cloudinary storage: {ex.Message}", ex);
	}
  }

  public async Task<Stream> DownloadFileStream(
	  string fileUrl,
	  CancellationToken cancellationToken = default)
  {
	if (string.IsNullOrWhiteSpace(fileUrl))
	  throw new CloudStorageException("Document storage URL is empty or invalid.");

	var client = _httpClientFactory.CreateClient();
	var response = await client.GetAsync(
		fileUrl,
		HttpCompletionOption.ResponseHeadersRead,
		cancellationToken);

	if (!response.IsSuccessStatusCode)
	  throw new CloudStorageException($"Failed to download document file: {response.ReasonPhrase}");

	return await response.Content.ReadAsStreamAsync(cancellationToken);
  }
}
