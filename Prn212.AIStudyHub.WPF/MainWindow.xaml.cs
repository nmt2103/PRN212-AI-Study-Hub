using System.Windows;
using Prn212.AIStudyHub.WPF.Views;

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
  }
}
