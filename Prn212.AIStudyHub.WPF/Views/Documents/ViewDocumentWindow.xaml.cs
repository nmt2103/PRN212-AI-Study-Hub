using Microsoft.Win32;
using Prn212.AIStudyHub.DataAccess;
using Prn212.AIStudyHub.Services.Documents;
using System.Windows;
using System.Windows.Controls;

namespace Prn212.AIStudyHub.WPF.Views.Documents
{
  /// <summary>
  /// Xem chi tiết tài liệu: hiển thị thông tin, download, edit, delete.
  /// NGƯỜI DOC làm ở file này để tích hợp vào DocumentListWindow sau.
  /// </summary>
  public partial class ViewDocumentWindow : Window
  {
    private readonly DocumentService _documentService = new();
    private Document? _currentDocument;

    public ViewDocumentWindow()
    {
      InitializeComponent();
      this.Loaded += ViewDocumentWindow_Loaded;
    }

    private void ViewDocumentWindow_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        LoadDocuments();
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Lỗi khi tải dữ liệu:\n{ex.Message}",
                        "Lỗi tải dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    /// <summary>
    /// Tải danh sách tài liệu từ service (không filter, trang 1, 100 item)
    /// </summary>
    private void LoadDocuments()
    {
      try
      {
        var (documents, totalCount) = _documentService.GetPaged(page: 1, pageSize: 100);
        cbDocuments.ItemsSource = documents;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Lỗi tải danh sách tài liệu:\n{ex.Message}",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    /// <summary>
    /// Event: Khi người dùng chọn tài liệu từ combobox
    /// </summary>
    private void CbDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (cbDocuments.SelectedItem is Document selectedDoc)
      {
        LoadDocumentDetail(selectedDoc.Id);
      }
      else
      {
        ClearDocumentDetail();
      }
    }

    /// <summary>
    /// Tải chi tiết tài liệu từ database và hiển thị
    /// </summary>
    private void LoadDocumentDetail(int documentId)
    {
      try
      {
        var doc = _documentService.GetDetail(documentId);
        if (doc == null)
        {
          MessageBox.Show("Tài liệu không tồn tại trên hệ thống.",
                          "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
          ClearDocumentDetail();
          return;
        }

        _currentDocument = doc;

        // Hiển thị thông tin chi tiết
        txtTitle.Text = doc.Title ?? "N/A";
        txtFileName.Text = doc.FileName ?? "N/A";
        txtFileSize.Text = FormatFileSize(doc.FileSize);
        txtFileExtension.Text = doc.FileExtension ?? "N/A";
        txtUploadedDate.Text = doc.UploadedAt.ToString("dd/MM/yyyy HH:mm:ss");
        txtSubject.Text = doc.Subject?.Name ?? "N/A";
        txtContentType.Text = doc.ContentType ?? "N/A";

        // Hiển thị thông tin tác giả
        string authorName = $"{doc.User?.FirstName} {doc.User?.LastName}".Trim();
        string authorEmail = doc.User?.Email ?? "N/A";
        txtAuthor.Text = $"{authorName} ({authorEmail})";

        // Enable action buttons
        btnDownload.IsEnabled = true;
        btnEdit.IsEnabled = true;
        btnDelete.IsEnabled = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Lỗi khi tải chi tiết tài liệu:\n{ex.Message}",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        ClearDocumentDetail();
      }
    }

    /// <summary>
    /// Xóa sạch thông tin chi tiết tài liệu khỏi giao diện
    /// </summary>
    private void ClearDocumentDetail()
    {
      _currentDocument = null;
      txtTitle.Clear();
      txtFileName.Clear();
      txtFileSize.Clear();
      txtFileExtension.Clear();
      txtUploadedDate.Clear();
      txtSubject.Clear();
      txtContentType.Clear();
      txtAuthor.Clear();

      btnDownload.IsEnabled = false;
      btnEdit.IsEnabled = false;
      btnDelete.IsEnabled = false;
    }

    /// <summary>
    /// Nút Download: Mở dialog lưu file, rồi gọi DownloadAsync từ service
    /// </summary>
    private async void BtnDownload_Click(object sender, RoutedEventArgs e)
    {
      if (_currentDocument == null)
      {
        MessageBox.Show("Vui lòng chọn tài liệu cần tải xuống!",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      // Mở dialog chọn nơi lưu file
      var saveDialog = new SaveFileDialog
      {
        FileName = _currentDocument.FileName,
        Filter = $"Tệp tin (*{_currentDocument.FileExtension})|*{_currentDocument.FileExtension}|Tất cả tệp tin (*.*)|*.*",
        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
      };

      if (saveDialog.ShowDialog() == true)
      {
        string destPath = saveDialog.FileName;

        // Tạo progress callback để cập nhật progress bar
        var progress = new Progress<double>(percent =>
        {
          pbProgress.Value = percent;
          txtStatus.Text = $"Đang tải xuống: {percent:F0}%";
        });

        try
        {
          // Hiển thị progress bar
          spProgress.Visibility = Visibility.Visible;
          btnDownload.IsEnabled = false;
          btnEdit.IsEnabled = false;
          btnDelete.IsEnabled = false;

          // Gọi service để download
          await _documentService.DownloadAsync(_currentDocument.Id, destPath, progress);

          MessageBox.Show($"Tải xuống tài liệu thành công!\nĐường dẫn: {destPath}",
                          "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Lỗi khi tải xuống tài liệu:\n{ex.Message}",
                          "Lỗi tải xuống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
          // Ẩn progress bar, enable buttons
          spProgress.Visibility = Visibility.Collapsed;
          pbProgress.Value = 0;
          txtStatus.Text = string.Empty;
          btnDownload.IsEnabled = true;
          btnEdit.IsEnabled = true;
          btnDelete.IsEnabled = true;
        }
      }
    }

    /// <summary>
    /// Nút Edit: Mở cửa sổ EditDocumentWindow để sửa metadata
    /// </summary>
    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
      if (_currentDocument == null)
      {
        MessageBox.Show("Vui lòng chọn tài liệu cần chỉnh sửa!",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      try
      {
        // Mở EditDocumentWindow
        var editWindow = new EditDocumentWindow();
        editWindow.Owner = this;
        editWindow.ShowDialog();

        // Sau khi sửa xong, tải lại chi tiết tài liệu
        LoadDocumentDetail(_currentDocument.Id);

        // Reload danh sách để thấy thay đổi
        LoadDocuments();
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Lỗi khi mở cửa sổ sửa:\n{ex.Message}",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    /// <summary>
    /// Nút Delete: Xóa tài liệu (xóa database + xóa file vật lý)
    /// </summary>
    private async void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
      if (_currentDocument == null)
      {
        MessageBox.Show("Vui lòng chọn tài liệu cần xóa!",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      // Xác nhận xóa
      var result = MessageBox.Show(
          $"Bạn có chắc chắn muốn xóa tài liệu:\n\"{_currentDocument.Title}\"?\n\n" +
          "Hành động này sẽ xóa dữ liệu từ database và file vật lý trên server.\n" +
          "Không thể hoàn tác!",
          "Xác nhận xóa tài liệu",
          MessageBoxButton.YesNo,
          MessageBoxImage.Warning);

      if (result == MessageBoxResult.Yes)
      {
        try
        {
          btnDownload.IsEnabled = false;
          btnEdit.IsEnabled = false;
          btnDelete.IsEnabled = false;

          // Gọi service để xóa
          await _documentService.DeleteAsync(_currentDocument.Id);

          MessageBox.Show("Xóa tài liệu thành công!",
                          "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

          // Tải lại danh sách
          LoadDocuments();
          ClearDocumentDetail();
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Lỗi khi xóa tài liệu:\n{ex.Message}",
                          "Lỗi xóa", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
          btnDownload.IsEnabled = _currentDocument != null;
          btnEdit.IsEnabled = _currentDocument != null;
          btnDelete.IsEnabled = _currentDocument != null;
        }
      }
    }

    /// <summary>
    /// Định dạng kích thước file từ bytes sang B/KB/MB/GB
    /// </summary>
    private string FormatFileSize(long bytes)
    {
      if (bytes == 0)
        return "0 B";

      string[] sizes = { "B", "KB", "MB", "GB", "TB" };
      double len = bytes;
      int order = 0;

      while (len >= 1024 && order < sizes.Length - 1)
      {
        order++;
        len = len / 1024;
      }

      return $"{len:F2} {sizes[order]}";
    }

    /// <summary>
    /// Nút Cancel: Đóng cửa sổ
    /// </summary>
    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
