using System.Windows;
using MySteamWPF.Core.Exceptions;
using MySteamWPF.Core.Services;
using MySteamWPF.Core.Utilities;

namespace MySteamWPF.Views;

/// <summary>
/// Interaction logic for the user registration window.
/// </summary>
public partial class RegisterWindow : Window
{
    public RegisterWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles registration logic after the user clicks the register button.
    /// Validates input and registers a new account.
    /// </summary>
    private void Register_Click(object sender, RoutedEventArgs e)
    {
        var login = LoginBox.Text.Trim();
        var email = EmailBox.Text.Trim();
        var name = NameBox.Text.Trim();
        var password = PasswordBox.Password;

        if (!Validator.IsValidLogin(login))
        {
            MessageBox.Show("Логин должен содержать минимум 3 символа и только буквы, цифры, тире и подчеркивания.");
            return;
        }

        if (!Validator.IsValidEmail(email))
        {
            MessageBox.Show("Некорректный email.");
            return;
        }

        if (!Validator.IsValidName(name))
        {
            MessageBox.Show("Имя слишком короткое.");
            return;
        }

        if (!Validator.IsValidPassword(password))
        {
            MessageBox.Show("Пароль должен быть не менее 8 символов и содержать буквы и цифры.");
            return;
        }

        try
        {
            AccountManager.Register(login, name, email, password);
            MessageBox.Show("Регистрация прошла успешно!");
            DialogResult = true;
            Close();
        }
        catch (UserExistsException ex)
        {
            Logger.LogException(ex, $"Attempt to register existing user: {login}, {email}");
            MessageBox.Show("Ошибка: " + ex.Message);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"Unexpected error during registration for: {login}, {email}");
            MessageBox.Show("Неизвестная ошибка: " + ex.Message);
        }
    }

    /// <summary>
    /// Cancels the registration and closes the window.
    /// </summary>
    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
