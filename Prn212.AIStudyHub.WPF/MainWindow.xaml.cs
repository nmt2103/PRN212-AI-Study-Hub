using Prn212.AIStudyHub.DataAccess;
using Prn212.AIStudyHub.Services.Documents;
using Prn212.AIStudyHub.WPF.Views;
using Prn212.AIStudyHub.WPF.Views.Auth;
using Prn212.AIStudyHub.WPF.Views.Documents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Prn212.AIStudyHub.WPF
{
  /// <summary>
  /// Trang chủ sau khi đăng nhập: hiển thị danh sách tài liệu, tìm/lọc/sắp xếp,
  /// mở màn hình upload, chi tiết, tải xuống, sửa, xóa, và cập nhật thông tin cá nhân.
  /// </summary>
  public partial class MainWindow : Window
  {
    private readonly DocumentService _documentService = new();
    private bool _initialized;
    private readonly DispatcherTimer _debounceTimer;

    public MainWindow()
    {
      InitializeComponent();
      Loaded += MainWindow_Loaded;

      _debounceTimer = new DispatcherTimer
      {
        Interval = TimeSpan.FromMilliseconds(300)
      };
      _debounceTimer.Tick += DebounceTimer_Tick;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      if (App.CurrentUser != null)
      {
        txtWelcome.Text = $"Xin chào, {App.CurrentUser.LastName} {App.CurrentUser.FirstName}";
        txtProfileButtonText.Text = $" {App.CurrentUser.FirstName}";
        txtProfileName.Text = $"{App.CurrentUser.LastName} {App.CurrentUser.FirstName}";
        txtProfileEmail.Text = App.CurrentUser.Email;
      }

      LoadSubjectFilter();
      _initialized = true;
      LoadDocuments();
    }

    private void LoadSubjectFilter()
    {
      var subjects = _documentService.GetAllSubjects();
      subjects.Insert(0, new Subject { Id = -1, Name = "-- Tất cả môn --" });
      cbSubjectFilter.ItemsSource = subjects;
      cbSubjectFilter.SelectedIndex = 0;
    }

    private void LoadDocuments()
    {
      try
      {
        string? keyword = string.IsNullOrWhiteSpace(txtSearch.Text) ? null : txtSearch.Text.Trim();

        int? subjectId = null;
        if (cbSubjectFilter.SelectedItem is Subject selectedSubject && selectedSubject.Id != -1)
        {
          subjectId = selectedSubject.Id;
        }

        string? sortBy = (cbSort.SelectedItem as ComboBoxItem)?.Tag?.ToString();

        // Tìm kiếm trên: Title, Subject Name, Subject Description
        var (items, total) = _documentService.SearchDocuments(keyword, subjectId, 1, 200, sortBy);
        dgDocuments.ItemsSource = items;

        if (items == null || items.Count == 0)
        {
          tbEmptyState.Visibility = Visibility.Visible;
        }
        else
        {
          tbEmptyState.Visibility = Visibility.Collapsed;
        }

        // Hiển thị trạng thái tìm kiếm
        string searchStatus = string.IsNullOrWhiteSpace(keyword)
            ? $"Hiển thị {items.Count} / tổng {total} tài liệu."
            : $"Hiển thị {items.Count} / tìm thấy {total} kết quả cho '{keyword}'.";
        txtStatus.Text = searchStatus;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Lỗi khi tải danh sách tài liệu:\n{ex.Message}",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void Filter_Changed(object sender, SelectionChangedEventArgs e)
    {
      if (_initialized)
        LoadDocuments();
    }

    private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        _debounceTimer.Stop();
        LoadDocuments();
      }
    }

    /// <summary>
    /// Real-time search với debounce 300ms
    /// Khi user gõ: delay 300ms trước khi search
    /// Nếu user gõ tiếp: reset timer
    /// Khi user dừng gõ 300ms: trigger search
    /// </summary>
    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
      _debounceTimer.Stop();
      _debounceTimer.Start();
    }

    private void DebounceTimer_Tick(object? sender, EventArgs e)
    {
      _debounceTimer.Stop();
      LoadDocuments();
    }

    private void BtnUpload_Click(object sender, RoutedEventArgs e)
    {
      var uploadWindow = new UploadDocumentWindow { Owner = this };
      uploadWindow.ShowDialog();
      LoadDocuments(); // làm mới danh sách sau khi upload xong
    }

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
      var confirm = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận đăng xuất",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
      if (confirm != MessageBoxResult.Yes)
        return;

      App.CurrentUser = null;        // xóa session
      var login = new LoginWindow();
      login.Show();                  // mở lại màn hình đăng nhập trước
      this.Close();                  // rồi đóng trang chủ
    }

    private void BtnOpenDetail_Click(object sender, RoutedEventArgs e)
    {
      if (dgDocuments.SelectedItem is Document selectedDoc)
      {
        var viewWindow = new ViewDocumentWindow(selectedDoc.Id) { Owner = this };
        viewWindow.ShowDialog();
        LoadDocuments(); // Tải lại danh sách sau khi quay về (nếu có xóa/sửa)
      }
    }

    private void DgDocuments_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (dgDocuments.SelectedItem is Document selectedDoc)
      {
        var viewWindow = new ViewDocumentWindow(selectedDoc.Id);
        viewWindow.Owner = this;
        viewWindow.ShowDialog();
        LoadDocuments(); // Tải lại danh sách sau khi quay về
      }
    }

    private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (sender is DataGridRow row)
      {
        row.IsSelected = true;
        row.Focus();
      }
    }

    private async void MenuItemDownload_Click(object sender, RoutedEventArgs e)
    {
      if (dgDocuments.SelectedItem is not Document selectedDoc)
      {
        MessageBox.Show("Vui lòng chọn tài liệu cần tải!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      var saveDialog = new Microsoft.Win32.SaveFileDialog
      {
        FileName = selectedDoc.FileName,
        Filter = $"Tệp tin (*{selectedDoc.FileExtension})|*{selectedDoc.FileExtension}|Tất cả các tệp (*.*)|*.*",
        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
      };

      if (saveDialog.ShowDialog() == true)
      {
        string destPath = saveDialog.FileName;
        try
        {
          txtStatus.Text = $"Đang tải xuống '{selectedDoc.Title}'...";
          await _documentService.DownloadAsync(selectedDoc.Id, destPath);
          MessageBox.Show("Tải xuống tài liệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Lỗi khi tải xuống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
          LoadDocuments();
        }
      }
    }


    private async void MenuItemDelete_Click(object sender, RoutedEventArgs e)
    {
      if (dgDocuments.SelectedItem is not Document selectedDoc)
      {
        MessageBox.Show("Vui lòng chọn tài liệu cần xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      var result = MessageBox.Show(
          $"Bạn có chắc chắn muốn xóa tài liệu '{selectedDoc.Title}' không?\nHành động này không thể hoàn tác.",
          "Xác nhận xóa tài liệu",
          MessageBoxButton.YesNo,
          MessageBoxImage.Warning);

      if (result == MessageBoxResult.Yes)
      {
        if (App.CurrentUser == null)
        {
          MessageBox.Show("Không tìm thấy thông tin tài khoản đăng nhập!", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }

        try
        {
          txtStatus.Text = $"Đang xóa tài liệu '{selectedDoc.Title}'...";
          await _documentService.DeleteAsync(selectedDoc.Id, App.CurrentUser.Id);
          MessageBox.Show("Xóa tài liệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Xóa thất bại: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
          LoadDocuments();
        }
      }
    }

    private void BtnOpenEdit_Click(object sender, RoutedEventArgs e)
    {
      if (dgDocuments.SelectedItem is Document selectedDoc)
      {
        var editWindow = new Views.Documents.EditDocumentWindow(selectedDoc.Id) { Owner = this };
        editWindow.ShowDialog();
        LoadDocuments();
      }
    }

    private void btnUpdateProfile_Click(object sender, RoutedEventArgs e)
    {
      var updateWindow = new Views.Account.UpdateProfileWindow();
      updateWindow.Owner = this;
      updateWindow.ShowDialog();
    }

    private void btnProfile_Click(object sender, RoutedEventArgs e)
    {
      ProfilePopup.IsOpen = !ProfilePopup.IsOpen;
    }

    private void ContextMenu_Opened(object sender, RoutedEventArgs e)
    {
      if (dgDocuments.SelectedItem is Document selectedDoc && App.CurrentUser != null)
      {
        bool isOwner = selectedDoc.UserId == App.CurrentUser.Id;
        if (sender is ContextMenu menu)
        {
          foreach (var item in menu.Items)
          {
            if (item is MenuItem mi)
            {
              string header = mi.Header?.ToString() ?? string.Empty;
              if (header == "Chỉnh sửa" || header == "Xóa")
              {
                mi.IsEnabled = isOwner;
              }
            }
          }
        }
      }
    }
  }
}
