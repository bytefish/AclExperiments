﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

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
        [SetUp]
        protected async Task SetupAsync()
        {
            await OnSetupBeforeCleanupAsync();

            using (var context = await _services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContextAsync())
            {
                await context.Database.ExecuteSqlRawAsync("EXEC [Identity].[usp_Database_ResetForTests]");
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

        protected ApplicationDbContext BuildDbContext()
        {
            return _services.GetRequiredService<ApplicationDbContext>();
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

            services.AddDbContextFactory<ApplicationDbContext>(x =>
            {
                var connectionString = configuration.GetConnectionString("ApplicationDatabase");

                if (connectionString == null)
                {
                    throw new InvalidOperationException($"No Connection String named 'ApplicationDatabase' found in appsettings.json");
                }

                x.UseSqlServer("ApplicationDatabase");
            });

            return services.BuildServiceProvider();
        }

        public abstract void RegisterServices(IServiceCollection services);
    }
}