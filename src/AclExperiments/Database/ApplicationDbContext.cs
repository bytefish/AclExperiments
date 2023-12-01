// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace AclExperiments.Database
{
    /// <summary>
    /// The <see cref="DbContext"/> to query for <see cref="SqlRelationTuple"/> and <see cref="SqlNamespaceConfiguration"/>.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the DbSet to query for <see cref="SqlRelationTuple"/>.
        /// </summary>
        public DbSet<SqlRelationTuple> SqlRelationTuples { get; set; }

        /// <summary>
        /// Gets or sets the DbSet to query for <see cref="SqlNamespaceConfiguration"/>.
        /// </summary>
        public DbSet<SqlNamespaceConfiguration> SqlNamespaceConfigurations { get; set; }
    }
}
