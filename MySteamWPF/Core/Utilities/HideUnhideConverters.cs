using System.Globalization;
using System.Windows.Data;
using MySteamWPF.Core.Exceptions;

namespace MySteamWPF.Core.Utilities;

/// <summary>
/// Converts a view state tag (e.g., "ShowHidden") to a corresponding button label ("Скрыть" or "Показать").
/// Typically used to toggle the visibility of games in the user dashboard.
/// </summary>
public class HideUnhideConverter : IValueConverter
{
    /// <summary>
    /// Converts a tag value (e.g., "ShowHidden") to a localized button label.
    /// Returns "Показать" if the tag is "ShowHidden", otherwise returns "Скрыть".
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() == "ShowHidden" ? "Показать" : "Скрыть";
    }

    /// <summary>
    /// Not implemented. This converter is intended for one-way binding only.
    /// Throws <see cref="ConverterException"/> if used in reverse.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new ConverterException("HideUnhideConverter can only be used one");
    }
}