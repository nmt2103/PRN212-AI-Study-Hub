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
      var uploadWindow = new Views.UploadDocumentWindow();
      uploadWindow.Owner = this;
      uploadWindow.ShowDialog();
    }
  }
}