// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AclExperiments.Tests.Infrastructure
{
    /// <summary>
    /// Will be used by all integration tests.
    /// </summary>
    public abstract class IntegrationTestBase
    {
        /// <summary>
        /// Shared Configuration for all tests.
        /// </summary>
        protected readonly IConfiguration _configuration;

        /// <summary>
        /// Shared Services for all tests.
        /// </summary>
        protected readonly IServiceProvider _services;

        /// <summary>
        /// Shared DbContext Factory.
        /// </summary>
        protected  IDbContextFactory<ApplicationDbContext> _dbContextFactory = null!;

        /// <summary>
        /// Constructor.
        /// </summary>
        public IntegrationTestBase()
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
        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            await OnSetupBeforeCleanupAsync();

            _dbContextFactory = _services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();

            using (var context = await _dbContextFactory.CreateDbContextAsync(default).ConfigureAwait(false))
            {
                await context.Database
                    .ExecuteSqlRawAsync("EXEC [Identity].[usp_Database_ResetForTests]")
                    .ConfigureAwait(false);
            }
            
            await OnSetupAfterCleanupAsync();
        }

        /// <summary>
        /// Called before the transaction starts.
        /// </summary>
        protected virtual Task OnSetupBeforeCleanupAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called inside the transaction.
        /// </summary>
        protected virtual Task OnSetupAfterCleanupAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Builds an <see cref="ApplicationDbContext"/> based on a given Configuration. We 
        /// expect the Configuration to have a Connection String "ApplicationDatabase" to 
        /// be defined.
        /// </summary>
        /// <param name="configuration">A configuration provided by the appsettings.json</param>
        /// <returns>An initialized <see cref="ApplicationDbContext"/></returns>
        /// <exception cref="InvalidOperationException">Thrown when no Connection String "ApplicationDatabase" was found</exception>
        private IServiceProvider BuildServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.AddDbContextFactory<ApplicationDbContext>(options =>
            {
                var connectionString = _configuration.GetConnectionString("ApplicationDatabase");

                if (connectionString == null)
                {
                    throw new InvalidOperationException($"No Connection String named 'ApplicationDatabase' found in appsettings.json");
                }

                options
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine)
                    .UseSqlServer(connectionString);
            });

            services.AddLogging();

            RegisterServices(services);

            return services.BuildServiceProvider();
        }

        public abstract void RegisterServices(IServiceCollection services);
    }
}