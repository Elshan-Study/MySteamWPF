using System.Windows;

namespace MySteamWPF.Views;

public partial class ChangePasswordWindow : Window
{
    public string OldPassword => OldPasswordBox.Password;
    public string NewPassword => NewPasswordBox.Password;

    public bool IsConfirmed { get; private set; } = false;

    public ChangePasswordWindow()
    {
        InitializeComponent();
    }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        IsConfirmed = true;
        this.Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
