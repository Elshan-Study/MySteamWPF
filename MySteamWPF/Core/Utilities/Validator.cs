using System.Text.RegularExpressions;

namespace MySteamWPF.Core.Utilities;

/// <summary>
/// Provides methods for validating user input such as login, email, name, and password.
/// </summary>
public static class Validator
{
    /// <summary>
    /// Validates a user login. Must be at least 3 characters and contain only letters, numbers, underscores or dashes.
    /// </summary>
    public static bool IsValidLogin(string? login)
    {
        return !string.IsNullOrWhiteSpace(login)
               && Regex.IsMatch(login, @"^[a-zA-Z0-9_-]{3,}$");
    }

    /// <summary>
    /// Validates an email address. Must contain '@' and '.' in appropriate positions.
    /// </summary>
    public static bool IsValidEmail(string? email)
    {
        return !string.IsNullOrWhiteSpace(email)
               && Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    /// <summary>
    /// Validates a display name. Must not be empty and at least 2 characters.
    /// </summary>
    public static bool IsValidName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name)
               && name.Length >= 2;
    }

    /// <summary>
    /// Validates a password. Must be at least 8 characters and contain letters and digits.
    /// </summary>
    public static bool IsValidPassword(string? password)
    {
        return !string.IsNullOrWhiteSpace(password)
               && password.Length >= 8
               && Regex.IsMatch(password, @"[A-Za-z]")
               && Regex.IsMatch(password, @"\d");
    }
}