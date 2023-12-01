﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Database;
using AclExperiments.Expressions;
using AclExperiments.Parser;
using Microsoft.EntityFrameworkCore;

namespace AclExperiments.Stores
{
    /// <summary>
    /// EntityFramework-core based implementation of a <see cref="INamespaceConfigurationStore"/>.
    /// </summary>
    public class SqlNamespaceConfigurationStore : INamespaceConfigurationStore
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public SqlNamespaceConfigurationStore(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<NamespaceUsersetExpression> GetLatestNamespaceConfigurationAsync(string name, CancellationToken cancellationToken)
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                var latestNamespaceConfiguration = await context.SqlNamespaceConfigurations
                    .Where(x => x.Name == name)
                    .OrderByDescending(x => x.Version)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

                if (latestNamespaceConfiguration == null)
                {
                    throw new InvalidOperationException($"No Namespace Configuration named '{name}' found");
                }

                return NamespaceUsersetRewriteParser.Parse(latestNamespaceConfiguration.Content);
            }
        }

        public async Task<NamespaceUsersetExpression> GetNamespaceConfigurationAsync(string name, int version, CancellationToken cancellationToken)
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                var namespaceConfigurationByVersion = await context.SqlNamespaceConfigurations
                    .Where(x => x.Name == name)
                    .Where(x => x.Version == version)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

                if (namespaceConfigurationByVersion == null)
                {
                    throw new InvalidOperationException($"No Namespace Configuration with Name = '{name}' and Version = '{version}' found");
                }

                return NamespaceUsersetRewriteParser.Parse(namespaceConfigurationByVersion.Content);
            }
        }
    }
}