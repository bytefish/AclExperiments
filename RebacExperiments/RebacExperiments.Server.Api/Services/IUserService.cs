// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Server.Api.Infrastructure.Database;
using System.Security.Claims;

namespace RebacExperiments.Server.Api.Services
{
    /// <summary>
    /// An <see cref="IUserService"/> is responsible to get all claims associated with a user.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets the Claims for a given username and password.
        /// </summary>
        /// <param name="context">ApplicationDbContext</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="ServiceResult"/> with the associated claims, if successful</returns>
        Task<List<Claim>> GetClaimsAsync(ApplicationDbContext context, string username, string password, CancellationToken cancellationToken);
    }
}
