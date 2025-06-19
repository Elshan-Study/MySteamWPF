using System.IO;

namespace MySteamWPF.Core.Utilities;

/// <summary>
/// Provides utility methods for working with file system paths in the application.
/// </summary>
public static class PathHelper
{
    /// <summary>
    /// Resolves a relative file path to an absolute path based on the application's base directory.
    /// Returns an empty string if the input path is null or empty.
    /// </summary>
    /// <param name="relativePath">The relative path to resolve.</param>
    /// <returns>The absolute path to the file or directory.</returns>
    public static string ResolvePath(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
            return string.Empty;
        
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(baseDir, relativePath);
    }
}