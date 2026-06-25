using System.Windows;

namespace Prn212.AIStudyHub.WPF
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    /// <summary>
    /// Lưu trữ thông tin người dùng đang đăng nhập vào hệ thống
    /// </summary>
    public static Entities.AppUser? CurrentUser { get; set; }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      try
      {
        using (var context = new Entities.AistudyHubDbContext())
        {
          // Giả lập tài khoản đang đăng nhập là ThuanNM (hoặc lấy tài khoản đầu tiên nếu không tìm thấy)
          CurrentUser = context.AppUsers.FirstOrDefault(u => u.Email == "thuannm@aistudyhub.com")
                        ?? context.AppUsers.FirstOrDefault();
        }
      }
      catch (Exception ex)
      {
        // Ghi nhận lỗi kết nối DB nhưng không crash app ngay
        System.Diagnostics.Debug.WriteLine($"Lỗi khởi tạo session giả lập: {ex.Message}");
      }
    }
  }

}
