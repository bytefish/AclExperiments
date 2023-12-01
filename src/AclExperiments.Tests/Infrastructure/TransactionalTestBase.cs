// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments;
using AclExperiments.Database;
using AclExperiments.Stores;
using LayeredArchitecture.Shared.Data.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Transactions;

namespace AclExperiments.Tests.Infrastructure
{
    /// <summary>
    /// Will be used by all integration tests.
    /// </summary>
    public class TransactionalTestBase
    {
        /// <summary>
        /// Shared Configuration for all tests.
        /// </summary>
        protected readonly IConfiguration _configuration;

        /// <summary>
        /// Shared Services for all tests.
        /// </summary>
        protected readonly IServiceCollection _services;

        /// <summary>
        /// <see cref="TransactionScope"/> that all database calls enlist to
        /// </summary>
        private TransactionScope _transactionScope;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TransactionalTestBase()
        {
            _configuration = BuildConfiguration();
            _services = BuildServices(_configuration);
        }

        /// <summary>
        /// Read the appsettings.json for the Test.
        /// </summary>
        /// <returns></returns>
        private IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        /// <summary>
        /// The SetUp called by NUnit to start the transaction.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        [SetUp]
        protected void Setup()
        {
            OnSetupBeforeTransaction();
            _transactionScope = TransactionScopeManager.CreateTransactionScope(new TransactionSettings());
            OnSetupInTransaction();
        }

        /// <summary>
        /// The TearDown called by NUnit to rollback the transaction.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        [TearDown]
        protected void Teardown()
        {
            OnTearDownInTransaction();
            _transactionScope.Dispose(); // Rolls back all changes ...
            OnTearDownAfterTransaction();
        }

        /// <summary>
        /// Called before the transaction starts.
        /// </summary>
        public virtual void OnSetupBeforeTransaction()
        {
        }

        /// <summary>
        /// Called inside the transaction.
        /// </summary>
        public virtual void OnSetupInTransaction()
        {
        }

        /// <summary>
        /// Called before rolling back the transaction.
        /// </summary>
        public virtual void OnTearDownInTransaction()
        {

        }

        /// <summary>
        /// Called after transaction has been rolled back.
        /// </summary>
        public virtual void OnTearDownAfterTransaction()
        {
        }

        /// <summary>
        /// Builds an <see cref="ApplicationDbContext"/> based on a given Configuration. We 
        /// expect the Configuration to have a Connection String "ApplicationDatabase" to 
        /// be defined.
        /// </summary>
        /// <param name="configuration">A configuration provided by the appsettings.json</param>
        /// <returns>An initialized <see cref="ApplicationDbContext"/></returns>
        /// <exception cref="InvalidOperationException">Thrown when no Connection String "ApplicationDatabase" was found</exception>
        private IServiceCollection BuildServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.AddDbContextFactory<ApplicationDbContext>(x =>
            {
                var connectionString = configuration.GetConnectionString("ApplicationDatabase");

                if (connectionString == null)
                {
                    throw new InvalidOperationException($"No Connection String named 'ApplicationDatabase' found in appsettings.json");
                }

                x.UseSqlServer("ApplicationDatabase");
            });

            services.AddSingleton<INamespaceConfigurationStore, SqlNamespaceConfigurationStore>();
            services.AddSingleton<IRelationTupleStore, SqlRelationTupleStore>();
            services.AddSingleton<AclService>();

            return services;
        }
    }
}