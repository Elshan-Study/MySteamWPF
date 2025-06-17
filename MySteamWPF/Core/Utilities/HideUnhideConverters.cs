using System.Globalization;
using System.Windows.Data;
using MySteamWPF.Core.Exceptions;

namespace MySteamWPF.Core.Utilities;

public class HideUnhideConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() == "ShowHidden" ? "Показать" : "Скрыть";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new ConverterException("HideUnhideConverter can only be used one");
    }
}