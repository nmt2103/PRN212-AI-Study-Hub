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
        private DispatcherTimer? _debounceTimer;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
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
                    tbEmptyState.Visibility= Visibility.Collapsed;
                }

                // Hiển thị trạng thái tìm kiếm
                string searchStatus = string.IsNullOrWhiteSpace(keyword)
                    ? $"Hiển thị {items.Count} / tổng {total} tài liệu."
                    : $"Tìm thấy {total} kết quả cho '{keyword}'.";
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

        private void BtnSearch_Click(object sender, RoutedEventArgs e) => LoadDocuments();

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoadDocuments();
        }

        /// <summary>
        /// Real-time search với debounce 300ms
        /// Khi user gõ: delay 300ms trước khi search
        /// Nếu user gõ tiếp: reset timer
        /// Khi user dừng gõ 300ms: trigger search
        /// </summary>
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Dừng timer cũ nếu còn đang chạy
            if (_debounceTimer != null)
            {
                _debounceTimer.Stop();
            }

            // Tạo timer mới - delay 300ms trước khi search
            _debounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _debounceTimer.Tick += (s, args) =>
            {
                _debounceTimer.Stop();
                LoadDocuments(); // Thực hiện search sau 300ms
            };
            _debounceTimer.Start();
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
            int? selectedId = null;
            if (dgDocuments.SelectedItem is Document selectedDoc)
            {
                selectedId = selectedDoc.Id;
            }

            var viewWindow = new ViewDocumentWindow(selectedId);
            viewWindow.Owner = this;
            viewWindow.ShowDialog();
            LoadDocuments(); // Tải lại danh sách sau khi quay về (nếu có xóa/sửa)
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

        private void BtnOpenDownload_Click(object sender, RoutedEventArgs e)
        {
            var downloadWindow = new Views.Documents.DownloadDocumentWindow();
            downloadWindow.Owner = this;
            downloadWindow.ShowDialog();
        }

        private void BtnOpenDelete_Click(object sender, RoutedEventArgs e)
        {
            var deleteWindow = new Views.Documents.DeleteDocumentWindow();
            deleteWindow.Owner = this;
            deleteWindow.ShowDialog();
        }

        private void BtnOpenEdit_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new Views.Documents.EditDocumentWindow();
            editWindow.Owner = this;
            editWindow.ShowDialog();
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
    }
}
