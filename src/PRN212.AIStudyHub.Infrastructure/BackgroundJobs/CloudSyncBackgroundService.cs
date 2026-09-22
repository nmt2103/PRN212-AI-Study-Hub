using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PRN212.AIStudyHub.Application.Interfaces;
using PRN212.AIStudyHub.Application.Services.Cloud;
using System;
using System.Collections.Generic;
using System.Text;

namespace PRN212.AIStudyHub.Infrastructure.BackgroundJobs
{
  public class CloudSyncBackgroundService : BackgroundService
  {
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<CloudSyncBackgroundService> _logger;

	public CloudSyncBackgroundService(IServiceScopeFactory scopeFactory, ILogger<CloudSyncBackgroundService> logger)
	{
	  _scopeFactory = scopeFactory;
	  _logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
			_logger.LogInformation("Cloud Sync Background Service is running...");
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await SyncDocumentsToCloudAsync(stoppingToken);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Lỗi nghiêm trọng khi đang chạy Background Sync.");
				}

				await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
			}
	}
		private async Task SyncDocumentsToCloudAsync(CancellationToken cancellationToken)
		{
			// Create scope to get Dbcontext and CloudStorageService
			using var scope = _scopeFactory.CreateScope();
			var dbcontext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
			var cloudStorageService = scope.ServiceProvider.GetRequiredService<ICloudStorageService>();

			// Find document in local
			var pendingDocuments = await dbcontext.Documents.Where(d => d.IsCloudStored == false && d.IsDeleted == false).ToListAsync(cancellationToken);

			if (!pendingDocuments.Any()) return;

			_logger.LogInformation("Đã tìm thấy {Count} file cần đồng bộ.", pendingDocuments.Count);

			foreach (var doc in pendingDocuments)
			{
				try
				{
					var localFilePath = doc.StoragePath;
					if (!File.Exists(localFilePath))
					{
						_logger.LogWarning("Không tìm thấy file vật lý ở Local: {Path}", localFilePath);
						continue;
					}
					// Read physics file
					using var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read);

					// Push to Cloudinary
					var uploadResult = await cloudStorageService.UploadRawFileAsync(fileStream, doc.FileName, cancellationToken);

					fileStream.Close();
					doc.IsCloudStored = true;
					doc.CloudPublicId = uploadResult.PublicId;
					doc.StoragePath = uploadResult.SecureUrl;

					File.Delete(localFilePath);
					_logger.LogInformation("Đồng bộ thành công file: {FileName}", doc.FileName);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Đồng bộ hóa thất bại cho Document ID: {DocId}", doc.Id);
				}
			}
			await dbcontext.SaveChangesAsync(cancellationToken);
		}
  }
}
