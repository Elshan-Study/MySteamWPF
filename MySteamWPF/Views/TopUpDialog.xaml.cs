using System;
using System.Globalization;
using System.Windows;

namespace MySteamWPF.Views
{
    public partial class TopUpDialog : Window
    {
        public decimal Amount { get; private set; }

        public TopUpDialog()
        {
            InitializeComponent();
            AmountTextBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AmountTextBox.Text.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result)
                && result > 0)
            {
                Amount = result;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректную положительную сумму.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}