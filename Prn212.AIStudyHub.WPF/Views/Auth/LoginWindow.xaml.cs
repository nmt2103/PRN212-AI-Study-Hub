using System.Windows;
using System.Windows.Input;
using Prn212.AIStudyHub.Services.Auth;

namespace Prn212.AIStudyHub.WPF.Views.Auth
{
  public partial class LoginWindow : Window
  {
    private readonly AuthService _authService = new();

    public LoginWindow()
    {
      InitializeComponent();
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
      HideError();

      string email = txtEmail.Text.Trim();
      string password = pwdPassword.Password;

      if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
      {
        ShowError("Vui lòng nhập đầy đủ email và mật khẩu.");
        return;
      }

      try
      {
        var user = _authService.Login(email, password);
        if (user == null)
        {
          ShowError("Email hoặc mật khẩu không đúng.");
          return;
        }

        // Đăng nhập thành công -> lưu session và mở trang chủ
        App.CurrentUser = user;

        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
      }
      catch (Exception ex)
      {
        ShowError($"Lỗi kết nối: {ex.Message}");
      }
    }

    private void LnkRegister_Click(object sender, MouseButtonEventArgs e)
    {
      var registerWindow = new RegisterWindow { Owner = this };
      registerWindow.ShowDialog();
    }

    private void Pwd_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter) BtnLogin_Click(sender, e);
    }

    private void ShowError(string message)
    {
      txtError.Text = message;
      txtError.Visibility = Visibility.Visible;
    }

    private void HideError() => txtError.Visibility = Visibility.Collapsed;
  }
}
