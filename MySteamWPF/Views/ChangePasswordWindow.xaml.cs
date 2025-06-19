using System.Windows;
using MySteamWPF.Core.Services;
using MySteamWPF.Core.Utilities;

namespace MySteamWPF.Views;

/// <summary>
/// A window that allows the user to change their password by entering the old and new password.
/// </summary>
public partial class ChangePasswordWindow : Window
{
    /// <summary>
    /// Gets the old password entered by the user.
    /// </summary>
    public string OldPassword => OldPasswordBox.Password;
    
    /// <summary>
    /// Gets the new password entered by the user.
    /// </summary>
    public string NewPassword => NewPasswordBox.Password;

    /// <summary>
    /// Indicates whether the user confirmed the password change.
    /// </summary>
    public bool IsConfirmed { get; private set; } = false;

    /// <summary>
    /// Initializes the password change window.
    /// </summary>
    public ChangePasswordWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the OK button click. Confirms the password change and closes the window.
    /// </summary>
    private void OnOk(object sender, RoutedEventArgs e)
    {
        IsConfirmed = true;
        Logger.Log($"{AccountManager.CurrentUser} confirmed password change.");
        this.Close();
    }

    /// <summary>
    /// Handles the Cancel button click. Closes the window without applying changes.
    /// </summary>
    private void OnCancel(object sender, RoutedEventArgs e)
    {
        Logger.Log($"{AccountManager.CurrentUser}  cancelled password change.");
        this.Close();
    }
}
