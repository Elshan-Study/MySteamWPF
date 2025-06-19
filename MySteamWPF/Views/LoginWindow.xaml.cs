using System.Windows;
using MySteamWPF.Core.Exceptions;
using MySteamWPF.Core.Services;
using MySteamWPF.Core.Utilities;

namespace MySteamWPF.Views;

/// <summary>
/// Interaction logic for the login window where users can enter their email/login and password.
/// </summary>
public partial class LoginWindow
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the login button click event. Attempts to log the user in using email or login.
    /// </summary>
    private void Login_Click(object sender, RoutedEventArgs e)
    {
        string id = IdentifierBox.Text.Trim();
        string password = PasswordBox.Password;

        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Поля не должны быть пустыми.");
            return;
        }

        try
        {
            try
            {
                AccountManager.LoginByEmail(id, password);
            }
            catch (UserSearchException)
            {
                AccountManager.LoginByUserLogin(id, password);
            }

            MessageBox.Show($"Добро пожаловать, {AccountManager.CurrentUser?.Name}!");
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"Login failed for input: {id}");
            MessageBox.Show("Ошибка входа: " + ex.Message);
        }
    }

    /// <summary>
    /// Handles the cancel button click event. Closes the window without logging in.
    /// </summary>
    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
