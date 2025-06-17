using System.Windows;
using MySteamWPF.Core.Exceptions;
using MySteamWPF.Core.Services;

namespace MySteamWPF.Views;

public partial class LoginWindow
{
    public LoginWindow()
    {
        InitializeComponent();
    }

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
            // Попробуем найти по email, если не получилось — по логину
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
            MessageBox.Show("Ошибка входа: " + ex.Message);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
