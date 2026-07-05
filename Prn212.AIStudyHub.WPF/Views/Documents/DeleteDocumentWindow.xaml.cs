using Prn212.AIStudyHub.DataAccess;
using Prn212.AIStudyHub.Services.Documents;
using System.Windows;

namespace Prn212.AIStudyHub.WPF.Views.Documents
{
  public partial class DeleteDocumentWindow : Window
  {
    private readonly DocumentService _documentService = new();

    public DeleteDocumentWindow()
    {
      InitializeComponent();
      LoadDocuments();
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

    private async void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
      if (cbDocuments.SelectedItem is not Document selectedDoc)
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
        try
        {
          btnDelete.IsEnabled = false;

          await _documentService.DeleteAsync(selectedDoc.Id);

          MessageBox.Show("Xóa tài liệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

          // Nạp lại danh sách tài liệu
          LoadDocuments();
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Xóa thất bại: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
          btnDelete.IsEnabled = true;
        }
      }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }
  }
}
