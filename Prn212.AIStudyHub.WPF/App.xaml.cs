using System.Windows;
using Prn212.AIStudyHub.DataAccess;
using Prn212.AIStudyHub.Services.Auth;

namespace Prn212.AIStudyHub.WPF
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    /// <summary>Tài khoản đang đăng nhập (session).</summary>
    public static AppUser? CurrentUser { get; set; }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      try
      {
        // Giả lập đăng nhập bằng 1 tài khoản mẫu (sau này nhóm AUTH thay bằng màn hình login)
        CurrentUser = new AuthService().GetDefaultUser();
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"Lỗi khởi tạo session giả lập: {ex.Message}");
      }
    }
  }
}
