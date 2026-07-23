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
    /// Interaction logic for ResetPasswordWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow : Window
    {
        private readonly AccountService _accountService;
        public ResetPasswordWindow()
        {
            InitializeComponent();
            _accountService = new();
            if (App.CurrentUser != null)
            {
                txtEmail.Text = App.CurrentUser.Email;
            }
        }

        private async void btnChangePassword_click(object sender, RoutedEventArgs e)
        {
            string currentPassword = txtCurrentPassword.Password.Trim();
            string newPassword = pwNewPassword.Password.Trim();
            string confirmPw = pwConfirm.Password.Trim();

            if(string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPw))
            {
                MessageBox.Show("vui lòng không bỏ sót thông tin", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!confirmPw.Equals(newPassword))
            {
                MessageBox.Show("Mật khẩu không khớp", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                if (App.CurrentUser != null)
                {
                    await _accountService.RequestPasswordReset(App.CurrentUser.Email, newPassword, currentPassword);
                    MessageBox.Show("Cập nhật mật khẩu mới thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
