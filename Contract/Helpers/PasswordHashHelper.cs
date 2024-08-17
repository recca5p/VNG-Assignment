using System.Security.Cryptography;
using System.Text;

public static class PasswordHashHelper
{
    /// <summary>
    /// Hashes the provided password using SHA256.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password as a hexadecimal string.</returns>
    public static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            // Convert the password to a byte array
            byte[] bytes = Encoding.UTF8.GetBytes(password);

            // Compute the hash
            byte[] hash = sha256.ComputeHash(bytes);

            // Convert the hash to a hexadecimal string
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var b in hash)
            {
                stringBuilder.Append(b.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }

    /// <summary>
    /// Verifies if the provided password matches the hashed password.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>True if the password matches, otherwise false.</returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        // Hash the provided password
        string hashOfInput = HashPassword(password);

        // Compare the hashed password with the stored hashed password
        return hashOfInput.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
    }
}