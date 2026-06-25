using Microsoft.Win32;
using Prn212.AIStudyHub.WPF.Entities;
using System.IO;
using System.Windows;

namespace Prn212.AIStudyHub.WPF.Views
{
  /// <summary>
  /// Interaction logic for UploadDocumentWindow.xaml
  /// </summary>
  public partial class UploadDocumentWindow : Window
  {
    private readonly AistudyHubDbContext _context;
    private string _selectedFilePath = string.Empty;

    public UploadDocumentWindow()
    {
      InitializeComponent();
      _context = new AistudyHubDbContext();

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
        MessageBox.Show($"Lỗi khi tải dữ liệu khởi tạo từ database:\n{ex.Message}",
                        "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    /// <summary>
    /// Nạp dữ liệu danh sách Môn học từ Database và hiển thị tài khoản đăng nhập
    /// </summary>
    private void LoadComboBoxData()
    {
      // 1. Tải danh sách môn học (Subject)
      var subjects = _context.Subjects
          .OrderBy(s => s.Name)
          .ToList();
      cbSubject.ItemsSource = subjects;

      // Gợi ý chọn phần tử đầu tiên nếu có dữ liệu
      if (subjects.Any())
        cbSubject.SelectedIndex = 0;

      // 2. Hiển thị thông tin người dùng đang đăng nhập (tên đầy đủ + email)
      if (App.CurrentUser != null)
      {
        txtUserDisplayName.Text = $"{App.CurrentUser.LastName} {App.CurrentUser.FirstName} ({App.CurrentUser.Email})";
      }
      else
      {
        txtUserDisplayName.Text = "Chưa có thông tin tài khoản đang đăng nhập";
      }
    }

    /// <summary>
    /// Xử lý chọn file
    /// Mở hộp thoại OpenFileDialog, lọc các định dạng được phép và hiển thị thông tin tệp.
    /// </summary>
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

        // Hiển thị tên file và kích thước
        lblFileName.Text = $"{fileInfo.Name} ({fileSizeInMb:F2} MB)";
        lblFileName.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.DarkGreen);

        if (string.IsNullOrWhiteSpace(txtTitle.Text))
        {
          txtTitle.Text = Path.GetFileNameWithoutExtension(fileInfo.Name);
        }
      }
    }

    /// <summary>
    /// Xử lý tải lên và lưu vào DB
    /// Validate dữ liệu, copy file sang thư mục local và lưu bản ghi thông qua EF Core.
    /// </summary>
    private async void BtnUpload_Click(object sender, RoutedEventArgs e)
    {
      // 1. Kiểm tra tính hợp lệ của dữ liệu đầu vào
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
        MessageBox.Show("Không tìm thấy thông tin tài khoản đăng nhập! Vui lòng kiểm tra lại.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      if (string.IsNullOrEmpty(_selectedFilePath) || !File.Exists(_selectedFilePath))
      {
        MessageBox.Show("Vui lòng chọn một tệp tin hợp lệ!", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      var fileInfo = new FileInfo(_selectedFilePath);
      long fileSizeBytes = fileInfo.Length;
      const long maxFileSizeBytes = 50 * 1024 * 1024; // 50 MB

      // Kiểm tra dung lượng tệp
      if (fileSizeBytes > maxFileSizeBytes)
      {
        MessageBox.Show("Dung lượng tệp vượt quá giới hạn cho phép (Tối đa 50MB)!", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      string extension = Path.GetExtension(_selectedFilePath).ToLower();
      string[] allowedExtensions = { ".pdf", ".docx", ".xlsx", ".pptx", ".txt", ".md" };

      // Kiểm tra định dạng tệp tin
      if (!allowedExtensions.Contains(extension))
      {
        MessageBox.Show("Định dạng tệp không được hỗ trợ! Chỉ chấp nhận: .pdf, .docx, .xlsx, .pptx, .txt, .md", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      int selectedUserId = App.CurrentUser.Id;
      int selectedSubjectId = (int) cbSubject.SelectedValue;

      try
      {
        // Vô hiệu hóa nút và hiện ProgressBar để biểu thị đang xử lý
        SetUiEnabledState(false);

        // 2. Chuẩn bị thư mục cục bộ 'uploads'
        string uploadDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads");
        if (!Directory.Exists(uploadDir))
        {
          Directory.CreateDirectory(uploadDir);
        }

        // 3. Tạo tên tệp độc nhất để tránh bị đè tệp trùng tên
        string uniqueFileName = $"{Guid.NewGuid()}_{fileInfo.Name}";
        string destinationPath = Path.Combine(uploadDir, uniqueFileName);

        // 4. Sao chép tệp bất đồng bộ để tránh block UI thread
        using (FileStream sourceStream = File.OpenRead(_selectedFilePath))
        using (FileStream destStream = File.Create(destinationPath))
        {
          await sourceStream.CopyToAsync(destStream);
        }

        // 5. Thêm mới bản ghi vào Database thông qua DbContext
        var document = new Document
        {
          UserId = selectedUserId,
          SubjectId = selectedSubjectId,
          Title = txtTitle.Text.Trim(),
          FileName = fileInfo.Name,
          StoragePath = Path.Combine("uploads", uniqueFileName),
          FileSize = fileSizeBytes,
          FileExtension = extension,
          ContentType = GetContentType(extension),
          UploadedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        MessageBox.Show("Tải tài liệu lên và lưu thông tin thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

        this.Close();
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Đã xảy ra lỗi trong quá trình upload tài liệu:\n{ex.Message}", "Lỗi upload", MessageBoxButton.OK, MessageBoxImage.Error);
        SetUiEnabledState(true);
      }
    }

    /// <summary>
    /// Thay đổi trạng thái tương tác của UI trong lúc xử lý tác vụ IO
    /// </summary>
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

    /// <summary>
    /// Ánh xạ định dạng tệp sang MIME ContentType cơ bản
    /// </summary>
    private string GetContentType(string extension)
    {
      return extension.ToLower() switch
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

    /// <summary>
    /// Đóng cửa sổ khi người dùng bấm Hủy
    /// </summary>
    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Giải phóng kết nối DbContext khi đóng cửa sổ
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
      _context.Dispose();
    }
  }
}
