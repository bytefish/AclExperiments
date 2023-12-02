// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Expressions;

namespace AclExperiments.Stores
{
    public interface INamespaceConfigurationStore
    {
        /// <summary>
        /// Adds a new Namespace Configuration.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="version">Version Number</param>
        /// <param name="content">Content</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>Awaitable Task</returns>
        Task AddNamespaceConfigurationAsync(string name, int version, string content, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a Namespace Configuration by Name and Version.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="version">Version</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>Awaitable Task</returns>
        Task RemoveNamespaceConfigurationAsync(string name, int version, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the latest <see cref="NamespaceUsersetExpression"/> by its name.
        /// </summary>
        /// <param name="name">Namespace Name</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>The latest <see cref="NamespaceUsersetExpression"/></returns>
        Task<NamespaceUsersetExpression> GetLatestNamespaceConfigurationAsync(string name, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a <see cref="NamespaceUsersetExpression"/> by its name and version.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="version">Version</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>The Namespace Configuration by its name and version</returns>
        Task<NamespaceUsersetExpression> GetNamespaceConfigurationAsync(string name, int version, CancellationToken cancellationToken);
    }
}
