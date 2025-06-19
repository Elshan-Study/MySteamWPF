using System.Windows;
using System.Windows.Controls;
using MySteamWPF.Core.Services;
using MySteamWPF.Core.Utilities;

namespace MySteamWPF.Views;

/// <summary>
/// Top bar control that handles navigation and user session actions (login, logout, profile).
/// </summary>
public partial class TopBar : UserControl
{
    public TopBar()
    {
        InitializeComponent();
        UpdateVisibility();
    }

    /// <summary>
    /// Updates visibility of login/logout/profile buttons based on user login state.
    /// </summary>
    public void UpdateVisibility()
    {
        var loggedIn = AccountManager.CurrentUser != null;
        LoginButton.Visibility = loggedIn ? Visibility.Collapsed : Visibility.Visible;
        LogoutButton.Visibility = loggedIn ? Visibility.Visible : Visibility.Collapsed;
        ProfileButton.Visibility = loggedIn ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Navigates to the game catalogue.
    /// </summary>
    private void OnHomeClicked(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow).MainContentControl.Content = new GameCatalogue();
    }

    /// <summary>
    /// Opens the registration window. On success, informs user they can now log in.
    /// </summary>
    private void OnRegisterClicked(object sender, RoutedEventArgs e)
    {
        var registerWindow = new RegisterWindow { Owner = Application.Current.MainWindow };
        if (registerWindow.ShowDialog() != true) return;
        Logger.Log("User registered successfully.");
        MessageBox.Show("Теперь вы можете войти в аккаунт.");
    }

    /// <summary>
    /// Opens the login window. On success, shows user dashboard and updates button visibility.
    /// </summary>
    private void OnLoginClicked(object sender, RoutedEventArgs e)
    {
        var loginWindow = new LoginWindow { Owner = Application.Current.MainWindow };
        if (loginWindow.ShowDialog() != true) return;
        Logger.Log($"User {AccountManager.CurrentUser?.Login} logged in.");
        UpdateVisibility();
        ((MainWindow)Application.Current.MainWindow).MainContentControl.Content = new UserDashboard();
    }

    /// <summary>
    /// Logs out the current user and returns to the game catalogue.
    /// </summary>
    private void OnLogoutClicked(object sender, RoutedEventArgs e)
    {
        Logger.Log($"User {AccountManager.CurrentUser?.Login} logged out.");
        AccountManager.Logout();
        UpdateVisibility();
        MessageBox.Show("Вы вышли из аккаунта.");
        ((MainWindow)Application.Current.MainWindow).MainContentControl.Content = new GameCatalogue();
    }

    /// <summary>
    /// Navigates to the user's dashboard page.
    /// </summary>
    private void OnProfileClicked(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow).MainContentControl.Content = new UserDashboard();
    }
}
