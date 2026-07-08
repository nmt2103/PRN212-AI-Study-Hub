using Microsoft.Win32;
using Prn212.AIStudyHub.DataAccess;
using Prn212.AIStudyHub.Services.Documents;
using System.Windows;

namespace Prn212.AIStudyHub.WPF.Views.Documents
{
  public partial class DownloadDocumentWindow : Window
  {
    private readonly DocumentService _documentService = new();

    public DownloadDocumentWindow()
    {
      InitializeComponent();
      LoadDocuments();
    }

    private void LoadDocuments()
    {
      try
      {
        var result = _documentService.SearchDocuments(page: 1, pageSize: 100);
        cbDocuments.ItemsSource = result.Items;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Lỗi tải danh sách tài liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private async void BtnDownload_Click(object sender, RoutedEventArgs e)
    {
      if (cbDocuments.SelectedItem is not Document selectedDoc)
      {
        MessageBox.Show("Vui lòng chọn tài liệu cần tải!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      var dialog = new SaveFileDialog
      {
        FileName = selectedDoc.FileName,
        Filter = $"Tệp tin (*{selectedDoc.FileExtension})|*{selectedDoc.FileExtension}|Tất cả các tệp (*.*)|*.*"
      };

      if (dialog.ShowDialog() == true)
      {
        string destPath = dialog.FileName;
        var progressIndicator = new Progress<double>(percent =>
        {
          pbProgress.Value = percent;
          txtStatus.Text = $"Đang tải xuống: {percent:F0}%";
        });

        try
        {
          spProgress.Visibility = Visibility.Visible;
          btnDownload.IsEnabled = false;

          await _documentService.DownloadAsync(selectedDoc.Id, destPath, progressIndicator);

          MessageBox.Show("Tải xuống tài liệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Lỗi khi tải xuống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
          spProgress.Visibility = Visibility.Collapsed;
          btnDownload.IsEnabled = true;
        }
      }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }
  }
}
