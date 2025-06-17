using System.Security.Cryptography;

namespace MySteamWPF.Core.Utilities;

/// <summary>
/// Utility working with SHA256 and Pbkdf2 to hash passwords.
/// </summary>
public static class PasswordHasher
{
    private const int SaltSize = 16; // 128 bit
    private const int HashSize = 32; // 256 бit
    private const int Iterations = 100_000; // Safe value

    /// <summary>
    /// Hashes the password with salt and returns a format string: iterations.salt.hash
    /// <param name="password">Entered password</param>
    /// <returns>string type Hashcode with Pbkdf2</returns>
    /// </summary>
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Checks if a password matches a hash.
    /// <param name="password">Entered password</param>
    /// <param name="hashedPassword">Hash from database</param>
    /// <returns>true if the password is correct; otherwise false</returns>
    /// </summary>
    public static bool Verify(string? password, string hashedPassword)
    {
        try
        {
            var parts = hashedPassword.Split('.');
            if (parts.Length != 3) return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var hash = Convert.FromBase64String(parts[2]);

            if (password != null)
            {
                var inputHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    hash.Length);

                return CryptographicOperations.FixedTimeEquals(inputHash, hash);
            }
            else
            {
                return false;
            }
        }
        catch  (Exception ex)
        {
            Logger.LogException(ex, "PasswordHasher");
            return false;
        }
    }
}