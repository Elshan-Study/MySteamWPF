using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using MySteamWPF.Core.Exceptions;

namespace MySteamWPF.Core.Utilities;

/// <summary>
/// Converts a relative image file path to a <see cref="BitmapImage"/> for use in WPF bindings.
/// Useful for displaying user avatars or game images from local storage.
/// </summary>
public class ImagePathToBitmapConverter : IValueConverter
{
    /// <summary>
    /// Converts a relative file path to a fully initialized <see cref="BitmapImage"/>.
    /// Returns null if the path is null, empty, or the file does not exist.
    /// </summary>
    /// <param name="value">The relative image path as a string.</param>
    /// <param name="targetType">The target binding type (ignored).</param>
    /// <param name="parameter">Optional converter parameter (ignored).</param>
    /// <param name="culture">Culture info (ignored).</param>
    /// <returns>A <see cref="BitmapImage"/> if the file exists; otherwise null.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string path || string.IsNullOrWhiteSpace(path)) return null;

        var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        if (!File.Exists(fullPath)) return null;

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        return bitmap;
    }

    /// <summary>
    /// Not implemented. This converter supports only one-way conversion from path to image.
    /// </summary>
    /// <exception cref="ConverterException">Always thrown to indicate that reverse conversion is not supported.</exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => 
        throw new ConverterException("ImagePathToBitmapConverter.ConvertBack Error");
}