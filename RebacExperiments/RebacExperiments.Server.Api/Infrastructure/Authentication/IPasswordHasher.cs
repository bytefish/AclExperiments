// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Api.Infrastructure.Authentication
{
    /// <summary>
    /// Provides Password Hashing algorithms.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Generates a Password Hash using tge PBKDF2 algorithm.
        /// </summary>
        /// <param name="password">Cleartext-Password to generate Password Hash for</param>
        /// <returns>Base64-encoded Password Hash</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a hashed password against a provided password.
        /// </summary>
        /// <param name="hashedPassword">Hashed Password</param>
        /// <param name="providedPassword">Provided Password</param>
        /// <returns>true, if both passwords match; otherwise, false.</returns>
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}
