using System.Windows;
using Prn212.AIStudyHub.DataAccess;

namespace Prn212.AIStudyHub.WPF
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    /// <summary>
    /// Tài khoản đang đăng nhập. Được gán khi Login thành công, xóa (null) khi Đăng xuất.
    /// </summary>
    public static AppUser? CurrentUser { get; set; }
  }
}
