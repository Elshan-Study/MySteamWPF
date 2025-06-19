using System.Globalization;
using System.Windows;
using MySteamWPF.Core.Services;
using MySteamWPF.Core.Utilities;

namespace MySteamWPF.Views;

/// <summary>
/// Interaction logic for the balance top-up dialog.
/// </summary>
public partial class TopUpDialog : Window
{
    /// <summary>
    /// The amount of money the user wants to top up.
    /// </summary>
    public decimal Amount { get; private set; }

    public TopUpDialog()
    {
        InitializeComponent();
        AmountTextBox.Focus();
    }

    /// <summary>
    /// Handles the OK button click. Validates and parses the entered amount.
    /// </summary>
    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (decimal.TryParse(
                    AmountTextBox.Text.Replace(',', '.'),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var result) && result > 0)
            {
                Amount = result;
                Logger.Log($"User {AccountManager.CurrentUser?.Login} topped up {Amount:C}.");
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректную положительную сумму.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "Failed to parse top-up amount");
            MessageBox.Show("Произошла ошибка при обработке суммы пополнения.",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Handles the cancel button click.
    /// </summary>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

