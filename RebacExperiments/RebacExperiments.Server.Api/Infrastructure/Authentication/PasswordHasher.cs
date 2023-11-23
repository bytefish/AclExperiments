// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace RebacExperiments.Server.Api.Infrastructure.Authentication
{
    /// <summary>
    /// Secure Password Hashing based on ASP.NET Core Identity.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Random Number Generator.
        /// </summary>
        private static readonly RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

        private readonly ILogger<PasswordHasher> _logger;

        public PasswordHasher(ILogger<PasswordHasher> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Generates a Password Hash using tge PBKDF2 algorithm.
        /// </summary>
        /// <param name="password">Cleartext-Password to generate Password Hash for</param>
        /// <returns>Base64-encoded Password Hash</returns>
        public string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            const KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA512;
            const int iterCount = 100_000;
            const int saltSize = 128 / 8;
            const int numBytesRequested = 256 / 8;

            byte[] salt = new byte[saltSize];

            _randomNumberGenerator.GetBytes(salt);

            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];

            outputBytes[0] = 0x01; // format marker

            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, iterCount);
            WriteNetworkByteOrder(outputBytes, 9, saltSize);

            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);

            return Convert.ToBase64String(outputBytes);
        }

        /// <summary>
        /// Verifies a hashed password against a provided password.
        /// </summary>
        /// <param name="hashedPassword">Hashed Password</param>
        /// <param name="providedPassword">Provided Password</param>
        /// <returns>true, if both passwords match; otherwise, false.</returns>
        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null)
            {
                throw new ArgumentNullException(nameof(hashedPassword));
            }

            if (providedPassword == null)
            {
                throw new ArgumentNullException(nameof(providedPassword));
            }

            var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            try
            {
                // Read header information
                var prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPasswordBytes, 1);
                var iterCount = (int)ReadNetworkByteOrder(hashedPasswordBytes, 5);

                int saltLength = (int)ReadNetworkByteOrder(hashedPasswordBytes, 9);

                // Read the salt: must be >= 128 bits
                if (saltLength < 128 / 8)
                {
                    return false;
                }

                byte[] salt = new byte[saltLength];

                Buffer.BlockCopy(hashedPasswordBytes, 13, salt, 0, salt.Length);

                // Read the subkey (the rest of the payload): must be >= 128 bits
                int subkeyLength = hashedPasswordBytes.Length - 13 - salt.Length;

                if (subkeyLength < 128 / 8)
                {
                    return false;
                }

                byte[] expectedSubkey = new byte[subkeyLength];

                Buffer.BlockCopy(hashedPasswordBytes, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                // Hash the incoming password and verify it
                byte[] actualSubkey = KeyDerivation.Pbkdf2(providedPassword, salt, prf, iterCount, subkeyLength);

                return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
            }
            catch
            {
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.
                return false;
            }
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return (uint)buffer[offset + 0] << 24
                | (uint)buffer[offset + 1] << 16
                | (uint)buffer[offset + 2] << 8
                | buffer[offset + 3];
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }
    }
}
