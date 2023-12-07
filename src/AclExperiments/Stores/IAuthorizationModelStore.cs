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
        /// <returns>The latest <see cref="NamespaceUsersetExpression"/></returns>
        Task<AuthorizationModel> GetAuthorizationModelAsync(string modelKey, CancellationToken cancellationToken);
    }
}
