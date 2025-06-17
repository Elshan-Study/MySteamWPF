using System.Windows;
using MySteamWPF.Core.Exceptions;
using MySteamWPF.Core.Services;
using MySteamWPF.Core.Utilities;

namespace MySteamWPF.Views;

public partial class RegisterWindow : Window
{
    public RegisterWindow()
    {
        InitializeComponent();
    }

    private void Register_Click(object sender, RoutedEventArgs e)
    {
        var login = LoginBox.Text.Trim();
        var email = EmailBox.Text.Trim();
        var name = NameBox.Text.Trim();
        var password = PasswordBox.Password;

        if (!Validator.IsValidLogin(login))
        {
            MessageBox.Show("Логин должен содержать минимум 3 символа и только буквы, цифры, тире и " +
                            "подчеркивания.");
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
            MessageBox.Show("Ошибка: " + ex.Message);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Неизвестная ошибка: " + ex.Message);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
