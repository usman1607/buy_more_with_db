using System.Security.Cryptography;

namespace BuyMore.Helpers
{
    public static class UserHelper
    {
        const int hashSize = 32;
        const int iterations = 100_000;
        const string salt_base64 = "nsLu648nNdNA/DPKWYCqcw==";

        public static string EncryptPassword(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            var salt = Convert.FromBase64String(salt_base64);

            var hashed = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, hashSize);
            return Convert.ToBase64String(hashed);
        }

        public static bool IsValidPassword(string password, string encryptedPassword)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (encryptedPassword == null) throw new ArgumentNullException(nameof(encryptedPassword));

            try
            {
                var expectedHash = Convert.FromBase64String(encryptedPassword);

                var salt = Convert.FromBase64String(salt_base64);

                var hashed = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, hashSize);

                return CryptographicOperations.FixedTimeEquals(hashed, expectedHash);
            }
            catch
            {
                return false;
            }
        }
    }
}