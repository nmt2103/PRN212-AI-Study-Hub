using Prn212.AIStudyHub.DataAccess;
using Prn212.AIStudyHub.Services.Documents;
using System.Windows;
using System.Windows.Controls;

namespace Prn212.AIStudyHub.WPF.Views.Documents
{
  public partial class EditDocumentWindow : Window
  {
    private readonly DocumentService _documentService = new();

    public EditDocumentWindow()
    {
      InitializeComponent();
      LoadSubjects();
      LoadDocuments();
    }

    private void LoadSubjects()
    {
      try
      {
        var subjects = _documentService.GetAllSubjects();
        cbSubject.ItemsSource = subjects;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Lỗi tải danh sách môn học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void LoadDocuments()
    {
      try
      {
        var result = _documentService.GetPaged(1, 100);
        cbDocuments.ItemsSource = result.Items;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Lỗi tải danh sách tài liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void CbDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (cbDocuments.SelectedItem is Document selectedDoc)
      {
        txtTitle.Text = selectedDoc.Title;
        cbSubject.SelectedValue = selectedDoc.SubjectId;
      }
      else
      {
        txtTitle.Text = string.Empty;
        cbSubject.SelectedIndex = -1;
      }
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
      if (cbDocuments.SelectedItem is not Document selectedDoc)
      {
        MessageBox.Show("Vui lòng chọn tài liệu cần chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      string newTitle = txtTitle.Text;
      if (string.IsNullOrWhiteSpace(newTitle))
      {
        MessageBox.Show("Tiêu đề tài liệu không được để trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      if (cbSubject.SelectedValue is not int selectedSubjectId)
      {
        MessageBox.Show("Vui lòng chọn môn học cho tài liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      try
      {
        btnSave.IsEnabled = false;

        await _documentService.UpdateMetadataAsync(selectedDoc.Id, newTitle, selectedSubjectId);

        MessageBox.Show("Cập nhật thông tin tài liệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

        // Nạp lại danh sách tài liệu sau khi sửa thành công
        LoadDocuments();
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Cập nhật thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      finally
      {
        btnSave.IsEnabled = true;
      }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }
  }
}
