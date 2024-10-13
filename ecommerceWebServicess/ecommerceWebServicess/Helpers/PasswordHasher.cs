/***************************************************************************
 * File: PasswordHasher.cs
 * Description: This file implements password hashing and verification using 
 *              the PBKDF2 algorithm with cryptographically strong random salts.
 ***************************************************************************/

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace ecommerceWebServicess.Helpers
{
    // Helper class to handle password hashing and verification.
    public class PasswordHasher
    {
        // Hashes a given password using PBKDF2 with a random salt and returns the salt and hashed password as a string.
        public string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];

            // Generate a random salt for the password hash.
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Generate the password hash using the salt and PBKDF2 algorithm.
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Return the salt and hash concatenated in the format "salt:hashedPassword".
            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        // Verifies the provided password against the stored salt and hashed password.
        public bool VerifyPassword(string password, string storedPassword)
        {
            // Split the stored password to get the salt and the hash.
            var parts = storedPassword.Split(':');
            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = parts[1];

            // Hash the input password with the extracted salt.
            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Compare the computed hash with the stored hash.
            return hash == storedHash;
        }
    }
}
