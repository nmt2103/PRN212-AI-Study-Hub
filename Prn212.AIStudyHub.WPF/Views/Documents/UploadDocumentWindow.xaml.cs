using Microsoft.Win32;
using Prn212.AIStudyHub.Services.Documents;
using System.IO;
using System.Windows;

namespace Prn212.AIStudyHub.WPF.Views
{
  /// <summary>
  /// Interaction logic for UploadDocumentWindow.xaml
  /// Giao diện chỉ nhận thao tác người dùng và gọi Service - KHÔNG đụng database.
  /// </summary>
  public partial class UploadDocumentWindow : Window
  {
    private readonly DocumentService _documentService = new();
    private string _selectedFilePath = string.Empty;

    public UploadDocumentWindow()
    {
      InitializeComponent();
      this.Loaded += UploadDocumentWindow_Loaded;
    }

    private void UploadDocumentWindow_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        LoadComboBoxData();
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Lỗi khi tải dữ liệu khởi tạo:\n{ex.Message}",
                        "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void LoadComboBoxData()
    {
      var subjects = _documentService.GetAllSubjects();
      cbSubject.ItemsSource = subjects;
      if (subjects.Any())
        cbSubject.SelectedIndex = 0;

      if (App.CurrentUser != null)
        txtUserDisplayName.Text = $"{App.CurrentUser.LastName} {App.CurrentUser.FirstName} ({App.CurrentUser.Email})";
      else
        txtUserDisplayName.Text = "Chưa có thông tin tài khoản đang đăng nhập";
    }

    private void BtnBrowse_Click(object sender, RoutedEventArgs e)
    {
      var openFileDialog = new OpenFileDialog
      {
        Title = "Chọn tệp tài liệu học tập",
        Filter = "Tài liệu học tập (*.pdf;*.docx;*.xlsx;*.pptx;*.txt;*.md)|*.pdf;*.docx;*.xlsx;*.pptx;*.txt;*.md"
      };

      if (openFileDialog.ShowDialog() == true)
      {
        _selectedFilePath = openFileDialog.FileName;
        var fileInfo = new FileInfo(_selectedFilePath);
        double fileSizeInMb = (double) fileInfo.Length / (1024 * 1024);

        lblFileName.Text = $"{fileInfo.Name} ({fileSizeInMb:F2} MB)";
        lblFileName.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.DarkGreen);

        if (string.IsNullOrWhiteSpace(txtTitle.Text))
          txtTitle.Text = Path.GetFileNameWithoutExtension(fileInfo.Name);
      }
    }

    private async void BtnUpload_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(txtTitle.Text))
      {
        MessageBox.Show("Vui lòng nhập tiêu đề cho tài liệu!", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
        txtTitle.Focus();
        return;
      }
      if (cbSubject.SelectedValue == null)
      {
        MessageBox.Show("Vui lòng chọn môn học liên quan!", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }
      if (App.CurrentUser == null)
      {
        MessageBox.Show("Không tìm thấy thông tin tài khoản đăng nhập!", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      if (string.IsNullOrEmpty(_selectedFilePath) || !File.Exists(_selectedFilePath))
      {
        MessageBox.Show("Vui lòng chọn một tệp tin hợp lệ!", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      try
      {
        SetUiEnabledState(false);

        await _documentService.UploadAsync(
            App.CurrentUser.Id,
            (int) cbSubject.SelectedValue,
            txtTitle.Text.Trim(),
            _selectedFilePath);

        MessageBox.Show("Tải tài liệu lên và lưu thông tin thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        this.Close();
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Đã xảy ra lỗi trong quá trình upload tài liệu:\n{ex.Message}", "Lỗi upload", MessageBoxButton.OK, MessageBoxImage.Error);
        SetUiEnabledState(true);
      }
    }

    private void SetUiEnabledState(bool isEnabled)
    {
      txtTitle.IsEnabled = isEnabled;
      cbSubject.IsEnabled = isEnabled;
      txtUserDisplayName.IsEnabled = isEnabled;
      btnBrowse.IsEnabled = isEnabled;
      btnUpload.IsEnabled = isEnabled;
      btnCancel.IsEnabled = isEnabled;
      spProgress.Visibility = isEnabled ? Visibility.Collapsed : Visibility.Visible;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
