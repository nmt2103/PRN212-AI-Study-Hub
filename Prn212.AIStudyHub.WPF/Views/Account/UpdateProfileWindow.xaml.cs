using Prn212.AIStudyHub.DataAccess;
using Prn212.AIStudyHub.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
                dpCreateAt.SelectedDate = user.CreatedAt;
            } catch(Exception ex) 
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
            } catch(Exception ex)
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
