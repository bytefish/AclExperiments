// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Expressions;

namespace AclExperiments.Stores
{
    public interface IAuthorizationModelStore
    {
        /// <summary>
        /// Returns a <see cref="AuthorizationModel"/> with the given Model Key.
        /// </summary>
        /// <param name="modelKey">Unique Key to identify the model</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>The <see cref="AuthorizationModel"/> with the given key</returns>
        Task<AuthorizationModel> GetAuthorizationModelAsync(string modelKey, CancellationToken cancellationToken);

        /// <summary>
        /// Adds an <see cref="AuthorizationModel"/> to the database.
        /// </summary>
        /// <param name="authorizationModel">Authorization Model to store</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>Awaitable Task</returns>
        Task AddAuthorizationModelAsync(AuthorizationModel authorizationModel, CancellationToken cancellationToken);
    }
}
