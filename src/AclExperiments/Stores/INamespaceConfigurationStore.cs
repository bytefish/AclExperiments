using AclExperiment.CheckExpand.Expressions;
using AclExperiment.CheckExpand.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AclExperiment.CheckExpand.Stores
{
    public interface INamespaceConfigurationStore
    {
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
