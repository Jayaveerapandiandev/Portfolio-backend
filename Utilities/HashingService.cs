using System.Security.Cryptography;

namespace Portfolio_Api.Utilities
{
    public class HashingService
    {
        public string HashPassword(string password)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, 16, 100_000, HashAlgorithmName.SHA512);
            var salt = deriveBytes.Salt;
            var key = deriveBytes.GetBytes(32);
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(key)}";
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedKey = Convert.FromBase64String(parts[1]);

            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA512);
            var computedKey = deriveBytes.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(storedKey, computedKey);
        }
    }
}
