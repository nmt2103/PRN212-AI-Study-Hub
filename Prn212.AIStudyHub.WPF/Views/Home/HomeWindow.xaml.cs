using Prn212.AIStudyHub.WPF.Views.Auth;
using System.Windows;
using System.Windows.Input;

namespace Prn212.AIStudyHub.WPF.Views.Home
{
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
        }

        private void BtnGetStarted_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void LnkRegister_Click(object sender, MouseButtonEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            var registerWindow = new RegisterWindow { Owner = loginWindow };
            registerWindow.ShowDialog();

            this.Close();
        }
    }
}