using Prn212.AIStudyHub.DataAccess;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;

namespace Prn212.AIStudyHub.WPF.Views.Documents
{
  /// <summary>
  /// Interaction logic for PreviewDocumentWindow.xaml
  /// </summary>
  public partial class PreviewDocumentWindow : Window
  {
    private readonly Document _doc;

    public PreviewDocumentWindow(Document doc)
    {
      InitializeComponent();
      _doc = doc;
      LoadDocument(doc);
    }

    private void LoadDocument(Document doc)
    {
      if (doc == null)
      {
        ShowError("Không tìm thấy thông tin tài liệu.");
        return;
      }

      // 1. Cập nhật thông tin Header
      lblDocTitle.Text = doc.Title ?? "Không có tiêu đề";
      lblDocMeta.Text = $"{doc.FileExtension?.ToUpper() ?? "N/A"} • {FormatFileSize(doc.FileSize)}";

      string ext = (doc.FileExtension ?? "").ToLower();
      txtFileIcon.Text = ext switch
      {
        ".pdf" => "📕",
        ".docx" => "📘",
        ".xlsx" => "📗",
        ".pptx" => "📙",
        ".txt" => "📄",
        ".md" => "📝",
        _ => "📁"
      };

      // 2. Định vị đường dẫn tệp tin
      string filePath = Path.Combine(AppContext.BaseDirectory, doc.StoragePath);
      if (!File.Exists(filePath))
      {
        ShowError("Không tìm thấy tệp tin tài liệu trên hệ thống lưu trữ.");
        return;
      }

      // 3. Xử lý hiển thị theo định dạng
      try
      {
        if (ext == ".pdf")
        {
          pdfBorder.Visibility = Visibility.Visible;
          textViewerContainer.Visibility = Visibility.Collapsed;
          errorPanel.Visibility = Visibility.Collapsed;

          pdfViewer.Navigate(new Uri(filePath));
        }
        else if (ext == ".txt" || ext == ".md")
        {
          textViewerContainer.Visibility = Visibility.Visible;
          pdfBorder.Visibility = Visibility.Collapsed;
          errorPanel.Visibility = Visibility.Collapsed;

          string content = File.ReadAllText(filePath);
          flowDoc.Blocks.Clear();
          var p = new Paragraph(new Run(content)) { LineHeight = 24 };
          flowDoc.Blocks.Add(p);
        }
        else if (ext == ".docx")
        {
          textViewerContainer.Visibility = Visibility.Visible;
          pdfBorder.Visibility = Visibility.Collapsed;
          errorPanel.Visibility = Visibility.Collapsed;

          string content = ExtractTextFromDocx(filePath);
          flowDoc.Blocks.Clear();

          var paragraphs = content.Split(new[] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.None);
          foreach (var paraText in paragraphs)
          {
            if (!string.IsNullOrWhiteSpace(paraText))
            {
              var p = new Paragraph(new Run(paraText.Trim())) { Margin = new Thickness(0, 0, 0, 12), LineHeight = 24 };
              flowDoc.Blocks.Add(p);
            }
          }
        }
        else
        {
          ShowError($"Định dạng tệp tin {doc.FileExtension} không hỗ trợ xem trực tiếp.\nVui lòng tải xuống máy tính để xem.");
        }
      }
      catch (Exception ex)
      {
        ShowError($"Lỗi hệ thống khi tải tệp tin xem trước:\n{ex.Message}");
      }
    }

    private string ExtractTextFromDocx(string filePath)
    {
      try
      {
        using (var archive = ZipFile.OpenRead(filePath))
        {
          var entry = archive.GetEntry("word/document.xml");
          if (entry != null)
          {
            using (var stream = entry.Open())
            {
              var doc = XDocument.Load(stream);
              var w = (XNamespace) "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
              var paragraphs = doc.Descendants(w + "p")
                                  .Select(p => string.Concat(p.Descendants(w + "t").Select(t => t.Value)));
              return string.Join(Environment.NewLine + Environment.NewLine, paragraphs);
            }
          }
        }
      }
      catch (Exception ex)
      {
        return $"Không thể giải mã tệp Word: {ex.Message}";
      }
      return "Tệp Word không chứa dữ liệu văn bản hợp lệ.";
    }

    private void ShowError(string message)
    {
      pdfBorder.Visibility = Visibility.Collapsed;
      textViewerContainer.Visibility = Visibility.Collapsed;
      errorPanel.Visibility = Visibility.Visible;
      lblErrorMessage.Text = message;
    }

    private string FormatFileSize(long? bytes)
    {
      if (bytes == null)
        return "0 B";
      double size = bytes.Value;
      string[] suffix = { "B", "KB", "MB", "GB" };
      int index = 0;
      while (size >= 1024 && index < suffix.Length - 1)
      {
        size /= 1024;
        index++;
      }
      return $"{size:0.##} {suffix[index]}";
    }

    private void BtnDownload_Click(object sender, RoutedEventArgs e)
    {
      if (_doc == null)
        return;

      var saveDialog = new Microsoft.Win32.SaveFileDialog
      {
        FileName = _doc.FileName,
        Filter = $"Tệp tin (*{_doc.FileExtension})|*{_doc.FileExtension}|Tất cả tệp tin (*.*)|*.*",
        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
      };

      if (saveDialog.ShowDialog() == true)
      {
        try
        {
          string srcPath = Path.Combine(AppContext.BaseDirectory, _doc.StoragePath);
          File.Copy(srcPath, saveDialog.FileName, true);
          MessageBox.Show("Tải xuống tài liệu thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Lỗi khi tải xuống tài liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
