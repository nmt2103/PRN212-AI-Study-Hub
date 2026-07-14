using Prn212.AIStudyHub.DataAccess;
using Prn212.AIStudyHub.Services.Auth;
using System.Windows;

namespace Prn212.AIStudyHub.WPF.Views.Account
{
  /// <summary>
  /// Interaction logic for UpdateProfileWindow.xaml
  /// </summary>
  public partial class UpdateProfileWindow : Window
  {
    private readonly AccountService _accountService;
    public UpdateProfileWindow()
    {
      InitializeComponent();
      _accountService = new();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
      await LoadAccountProfile();
    }

    public async Task LoadAccountProfile()
    {
      try
      {
        AppUser user = await _accountService.GetCurrentUser("admin@aistudyhub.com");
        txtFullName.Text = $"{user.FirstName} {user.LastName}";
        txtEmail.Text = user.Email;
        txtFirstName.Text = user.FirstName;
        txtLastName.Text = user.LastName;
        txtCreatedAt.Text = user.CreatedAt.ToString("dd/MM/yyyy");
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        this.Close();
      }
    }
    private async void btnUpdateProfile_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var email = txtEmail.Text;
        var firstName = txtFirstName.Text;
        var lastName = txtLastName.Text;
        await _accountService.UpdateProfile(email, firstName, lastName);
        MessageBox.Show("Update successful", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        await LoadAccountProfile();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    }

    private void btnBack_click(object sender, RoutedEventArgs e)
    {
      Close();
    }
  }
}
