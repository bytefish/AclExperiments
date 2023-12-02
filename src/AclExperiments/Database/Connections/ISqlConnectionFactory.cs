// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Data.Common;

namespace AclExperiments.Database.Connections
{
    /// <summary>
    /// Creates a <see cref="DbConnection"/>.
    /// </summary>
    public interface ISqlConnectionFactory
    {
        /// <summary>
        /// Gets an opened <see cref="DbConnection"/>.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token to cancel asynchronous processing</param>
        /// <returns>An opened <see cref="DbConnection"/></returns>
        Task<DbConnection> GetDbConnectionAsync(CancellationToken cancellationToken);
    }
}