using System.Security.Cryptography;
using System.Text;

namespace AcademiaSys.Services
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            string salt = Convert.ToBase64String(saltBytes);

            // Combine salt with password and hash using SHA-256
            using (var sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Combine(Encoding.UTF8.GetBytes(password), saltBytes);
                byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                string hashedPassword = Convert.ToBase64String(hashedBytes);

                // Combine salt and hashed password into a single string
                return $"{salt}${hashedPassword}";
            }
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            // Extract salt from the stored hash
            string[] parts = storedHash.Split('$');
            if (parts.Length != 2)
            {
                // Handle invalid stored hash format
                return false;
            }

            string storedSalt = parts[0];
            string storedHashedPassword = parts[1];

            // Combine salt with provided password and hash using SHA-256
            using (var sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Combine(Encoding.UTF8.GetBytes(password), Convert.FromBase64String(storedSalt));
                byte[] hashedBytesToCheck = sha256.ComputeHash(combinedBytes);
                string hashedPasswordToCheck = Convert.ToBase64String(hashedBytesToCheck);

                return hashedPasswordToCheck == storedHashedPassword;
            }
        }

        private static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] result = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, result, 0, first.Length);
            Buffer.BlockCopy(second, 0, result, first.Length, second.Length);
            return result;
        }
    }
}
