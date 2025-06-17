using System.IO;

namespace MySteamWPF.Core.Utilities;

/// <summary>
/// Utility for logging messages and exceptions to a file.
/// </summary>
public static class Logger
{
    private const string LogFile = "log.txt";

    /// <summary>
    /// Writes a message to the log with the specified severity level.
    /// </summary>
    /// <param name="message">Text of message.</param>
    /// <param name="level">Logging level (Info, Warning, Error).</param>
    public static void Log(string message, string level = "Info")
    {
        var logEntry = $"{DateTime.Now:G} [{level}] {message}\n";
        File.AppendAllText(LogFile, logEntry);
    }

    /// <summary>
    /// Writes exception information to the log.
    /// </summary>
    /// <param name="ex">Exception that needs to be logged.</param>
    /// <param name="context">Additional description of the situation (optional).</param>
    public static void LogException(Exception ex, string? context = null)
    {
        var message = context != null ? $"{context}: {ex.Message}" : ex.Message;
        var stack = ex.StackTrace ?? "Not StackTrace.";
        var logEntry = $"{DateTime.Now:G} [Exception] {message}\n{stack}\n";
        File.AppendAllText(LogFile, logEntry);
    }
}