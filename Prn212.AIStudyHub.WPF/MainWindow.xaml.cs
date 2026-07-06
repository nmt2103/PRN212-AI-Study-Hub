using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Prn212.AIStudyHub.DataAccess;
using Prn212.AIStudyHub.Services.Documents;
using Prn212.AIStudyHub.WPF.Views;        // UploadDocumentWindow
using Prn212.AIStudyHub.WPF.Views.Auth;   // LoginWindow

namespace Prn212.AIStudyHub.WPF
{
  /// <summary>
  /// Trang chủ sau khi đăng nhập: hiển thị danh sách tài liệu, tìm/lọc/sắp xếp,
  /// mở màn hình upload và đăng xuất.
  /// </summary>
  public partial class MainWindow : Window
  {
    private readonly DocumentService _documentService = new();
    private bool _initialized;

    public MainWindow()
    {
      InitializeComponent();
      Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      if (App.CurrentUser != null)
        txtWelcome.Text = $"Xin chào, {App.CurrentUser.LastName} {App.CurrentUser.FirstName}";

      LoadSubjectFilter();
      _initialized = true;
      LoadDocuments();
    }

    private void LoadSubjectFilter()
    {
      var subjects = _documentService.GetAllSubjects();
      subjects.Insert(0, new Subject { Id = 0, Name = "-- Tất cả môn --" });
      cbSubjectFilter.ItemsSource = subjects;
      cbSubjectFilter.SelectedIndex = 0;
    }

    private void LoadDocuments()
    {
      try
      {
        string? keyword = string.IsNullOrWhiteSpace(txtSearch.Text) ? null : txtSearch.Text.Trim();

        int? subjectId = null;
        if (cbSubjectFilter.SelectedValue is int sid && sid > 0)
          subjectId = sid;

        string? sortBy = (cbSort.SelectedItem as ComboBoxItem)?.Tag?.ToString();

        var (items, total) = _documentService.GetPaged(1, 200, keyword, subjectId, sortBy);
        dgDocuments.ItemsSource = items;
        txtStatus.Text = $"Hiển thị {items.Count} / tổng {total} tài liệu.";
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Lỗi khi tải danh sách tài liệu:\n{ex.Message}",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void Filter_Changed(object sender, SelectionChangedEventArgs e)
    {
      if (_initialized) LoadDocuments();
    }

    private void BtnSearch_Click(object sender, RoutedEventArgs e) => LoadDocuments();

    private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter) LoadDocuments();
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
      if (confirm != MessageBoxResult.Yes) return;

      App.CurrentUser = null;        // xóa session
      var login = new LoginWindow();
      login.Show();                  // mở lại màn hình đăng nhập trước
      this.Close();                  // rồi đóng trang chủ
    }
  }
}
