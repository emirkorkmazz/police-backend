using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace Police.Helpers 
{
    public class PasswordHasher
    {
        public string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        public bool VerifyPassword(string hashedPasswordWithSalt, string providedPassword)
        {
            var parts = hashedPasswordWithSalt.Split('.');

            // Eğer salt ve hash ayrımı yapılamıyorsa, geçersiz giriş olarak kabul edin
            if (parts.Length != 2)
            {
                return false;
            }

            try
            {
                byte[] salt = Convert.FromBase64String(parts[0]);
                string hashedPassword = parts[1];

                string hashedProvidedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: providedPassword,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                return hashedPassword == hashedProvidedPassword;
            }
            catch (FormatException)
            {
                // Base64 format hatası, geçersiz giriş olarak kabul edilir
                return false;
            }
        }

    }
}
