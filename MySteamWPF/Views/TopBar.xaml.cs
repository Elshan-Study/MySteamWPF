using System.Windows;
using System.Windows.Controls;
using MySteamWPF.Core.Services;

namespace MySteamWPF.Views
{
    public partial class TopBar : UserControl
    {
        public TopBar()
        {
            InitializeComponent();
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            bool loggedIn = AccountManager.CurrentUser != null;
            LoginButton.Visibility = loggedIn ? Visibility.Collapsed : Visibility.Visible;
            LogoutButton.Visibility = loggedIn ? Visibility.Visible : Visibility.Collapsed;
            ProfileButton.Visibility = loggedIn ? Visibility.Visible : Visibility.Collapsed;
        }
        
        private void OnHomeClicked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContentControl.Content = new GameCatalogue();
        }

        private void OnRegisterClicked(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow { Owner = Application.Current.MainWindow };
            if (registerWindow.ShowDialog() == true)
            {
                MessageBox.Show("Теперь вы можете войти в аккаунт.");
            }
        }

        private void OnLoginClicked(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow { Owner = Application.Current.MainWindow };
            if (loginWindow.ShowDialog() == true)
            {
                UpdateVisibility();
                ((MainWindow)Application.Current.MainWindow).MainContentControl.Content = new UserProfilePage();
            }
        }

        private void OnLogoutClicked(object sender, RoutedEventArgs e)
        {
            AccountManager.Logout();
            UpdateVisibility();
            MessageBox.Show("Вы вышли из аккаунта.");
        }

        private void OnProfileClicked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContentControl.Content = new UserProfilePage();
        }
    }
}
