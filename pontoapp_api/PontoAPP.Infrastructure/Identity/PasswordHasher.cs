using System.Security.Cryptography;
using System.Text;

namespace PontoAPP.Infrastructure.Identity;

/// <summary>
/// Service for securely hashing and verifying passwords using PBKDF2
/// Uses industry-standard practices with salt and high iteration count
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 100000; // OWASP recommendation for 2024

    /// <summary>
    /// Hashes a password using PBKDF2 with a randomly generated salt
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Base64 encoded hash in format: {iterations}.{salt}.{hash}</returns>
    /// <exception cref="ArgumentException">When password is null or empty</exception>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        // Generate random salt
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        // Hash password with salt
        byte[] hash = HashPasswordWithSalt(password, salt, Iterations);

        // Return in format: iterations.salt.hash (all base64 encoded)
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Verifies a password against a hashed password
    /// </summary>
    /// <param name="password">Plain text password to verify</param>
    /// <param name="hashedPassword">Previously hashed password</param>
    /// <returns>True if password matches, false otherwise</returns>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        try
        {
            // Parse the hashed password
            var parts = hashedPassword.Split('.');
            if (parts.Length != 3)
                return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var hash = Convert.FromBase64String(parts[2]);

            // Hash the input password with same salt and iterations
            var hashToVerify = HashPasswordWithSalt(password, salt, iterations);

            // Compare hashes using constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(hash, hashToVerify);
        }
        catch
        {
            // If anything goes wrong (invalid format, etc), password is invalid
            return false;
        }
    }

    /// <summary>
    /// Validates password strength according to common security requirements
    /// </summary>
    /// <param name="password">Password to validate</param>
    /// <returns>Tuple with (isValid, errorMessage)</returns>
    public (bool IsValid, string? ErrorMessage) ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "Password cannot be empty");

        if (password.Length < 8)
            return (false, "Password must be at least 8 characters long");

        if (password.Length > 128)
            return (false, "Password must not exceed 128 characters");

        // Check for at least one uppercase letter
        if (!password.Any(char.IsUpper))
            return (false, "Password must contain at least one uppercase letter");

        // Check for at least one lowercase letter
        if (!password.Any(char.IsLower))
            return (false, "Password must contain at least one lowercase letter");

        // Check for at least one digit
        if (!password.Any(char.IsDigit))
            return (false, "Password must contain at least one digit");

        // Check for at least one special character
        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            return (false, "Password must contain at least one special character");

        return (true, null);
    }

    /// <summary>
    /// Internal method to hash password with salt using PBKDF2
    /// </summary>
    private static byte[] HashPasswordWithSalt(string password, byte[] salt, int iterations)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password: Encoding.UTF8.GetBytes(password),
            salt: salt,
            iterations: iterations,
            hashAlgorithm: HashAlgorithmName.SHA256);

        return pbkdf2.GetBytes(HashSize);
    }
}

/// <summary>
/// Interface for password hashing operations
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password securely
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against its hash
    /// </summary>
    bool VerifyPassword(string password, string hashedPassword);

    /// <summary>
    /// Validates password strength
    /// </summary>
    (bool IsValid, string? ErrorMessage) ValidatePasswordStrength(string password);
}