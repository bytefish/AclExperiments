// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RebacExperiments.Server.Api.Infrastructure.Database;
using System;
using System.Data;
using System.Threading.Tasks;

namespace RebacExperiments.Server.Api.Tests
{
    /// <summary>
    /// Will be used by all integration tests, that need an <see cref="ApplicationDbContext"/>.
    /// </summary>
    public class TransactionalTestBase
    {
        /// <summary>
        /// We can assume the Configuration has been initialized, when the Tests 
        /// are run. So we inform the compiler, that this field is intentionally 
        /// left uninitialized.
        /// </summary>
        protected IConfiguration _configuration = null!;

        /// <summary>
        /// We can assume the DbContext has been initialized, when the Tests 
        /// are run. So we inform the compiler, that this field is intentionally 
        /// left uninitialized.
        /// </summary>
        protected ApplicationDbContext _applicationDbContext = null!;

        public TransactionalTestBase()
        {
            _configuration = ReadConfiguration();
        }

        /// <summary>
        /// Read the appsettings.json for the Test.
        /// </summary>
        /// <returns></returns>
        private IConfiguration ReadConfiguration()
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
        protected async Task Setup()
        {
            // Create a fresh DbContext for each test, because you don't want the 
            // Change Tracker to cache entities and pollute the test.
            _applicationDbContext = GetApplicationDbContext(_configuration);

            await OnSetupBeforeTransaction();
            await _applicationDbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, default);
            await OnSetupInTransaction();
        }

        /// <summary>
        /// The TearDown called by NUnit to rollback the transaction.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        [TearDown]
        protected async Task Teardown()
        {
            await OnTearDownInTransaction();
            await _applicationDbContext.Database.RollbackTransactionAsync(default);
            await OnTearDownAfterTransaction();
            await _applicationDbContext.DisposeAsync();
        }

        /// <summary>
        /// Called before the transaction starts.
        /// </summary>
        /// <returns>An awaitable task</returns>
        public virtual Task OnSetupBeforeTransaction()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called inside the transaction.
        /// </summary>
        /// <returns>An awaitable task</returns>
        public virtual Task OnSetupInTransaction()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called before rolling back the transaction.
        /// </summary>
        /// <returns>An awaitable task</returns>
        public virtual Task OnTearDownInTransaction()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called after transaction has been rolled back.
        /// </summary>
        /// <returns>An awaitable task</returns>
        public virtual Task OnTearDownAfterTransaction()
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
        private ApplicationDbContext GetApplicationDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ApplicationDatabase");

            if (connectionString == null)
            {
                throw new InvalidOperationException($"No Connection String named 'ApplicationDatabase' found in appsettings.json");
            }

            return GetApplicationDbContext(connectionString);
        }

        /// <summary>
        /// Builds an <see cref="ApplicationDbContext"/> based on a given Connection String 
        /// and enables sensitive data logging for eventual debugging. 
        /// </summary>
        /// <param name="connectionString">Connection String to the Test database</param>
        /// <returns>An initialized <see cref="ApplicationDbContext"/></returns>
        private ApplicationDbContext GetApplicationDbContext(string connectionString)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(connectionString);

            return new ApplicationDbContext(
                logger: new NullLogger<ApplicationDbContext>(), 
                options: dbContextOptionsBuilder.Options);
        }
    }
}