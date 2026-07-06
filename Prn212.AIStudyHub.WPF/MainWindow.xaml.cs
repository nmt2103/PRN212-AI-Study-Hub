using Prn212.AIStudyHub.WPF.Views;
using System.Windows;

namespace Prn212.AIStudyHub.WPF
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void BtnOpenUpload_Click(object sender, RoutedEventArgs e)
    {
      var uploadWindow = new UploadDocumentWindow();
      uploadWindow.Owner = this;
      uploadWindow.ShowDialog();
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
