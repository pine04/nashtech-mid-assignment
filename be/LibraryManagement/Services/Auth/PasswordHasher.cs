using System.Security.Cryptography;

namespace LibraryManagement.Services.Auth
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int Iteration = 10000;
        private const int HashSize = 32;

        private readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteration, HashAlgorithm, HashSize);

            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }

        public bool Verify(string password, string storedPassword)
        {
            string[] components = storedPassword.Split("-");
            if (components.Length != 2)
            {
                return false;
            }

            byte[] originalHash = Convert.FromHexString(components[0]);
            byte[] salt = Convert.FromHexString(components[1]);

            byte[] newHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteration, HashAlgorithm, HashSize);

            return originalHash.SequenceEqual(newHash);
        }
    }
}