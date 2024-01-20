using System.Text;

namespace AcademiaSys.Services
{
    public class PasswordGenerator
    {
        private static readonly Random random = new Random();
        private const string alphanumericChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int passwordLength = 5;

        public static string GeneratePassword()
        {
            StringBuilder password = new StringBuilder();

            for (int i = 0; i < passwordLength; i++)
            {
                int index = random.Next(alphanumericChars.Length);
                password.Append(alphanumericChars[index]);
            }

            return password.ToString();
        }
    }
}
