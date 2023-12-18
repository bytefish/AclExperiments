// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace AclExperiments.Database.Connections
{
    /// <summary>
    /// SQL Server implementation for a <see cref="ISqlConnectionFactory"/>.
    /// </summary>
    public class SqlServerConnectionFactory : ISqlConnectionFactory
    {
        /// <summary>
        /// Gets or sets the Connection String.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the Retry Logic Provider.
        /// </summary>
        public SqlRetryLogicBaseProvider SqlRetryLogicProvider { get; set; }

        /// <summary>
        /// Creates a new <see cref="SqlServerConnectionFactory"/> based 
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlServerConnectionFactory(string connectionString)
            : this(connectionString, GetExponentialBackoffProvider(5, 1, 20))
        {
        }

        public SqlServerConnectionFactory(string connectionString, SqlRetryLogicBaseProvider sqlRetryLogicProvider)
        {
            // Enable Preview Features
            SetFeatureFlags();

            ConnectionString = connectionString;
            SqlRetryLogicProvider = sqlRetryLogicProvider;
        }

        public async Task<DbConnection> GetDbConnectionAsync(CancellationToken cancellationToken)
        {
            // Create the SqlConnection for the given connection string
            var dbConnection = new SqlConnection()
            {
                ConnectionString = ConnectionString,
                RetryLogicProvider = SqlRetryLogicProvider
            };

            await dbConnection.OpenAsync(cancellationToken);

            return dbConnection;
        }

        /// <summary>
        /// Creates a default Exponential Backoff Provider, 
        /// </summary>
        /// <param name="numberOfTries">Number of Retries for transient errors</param>
        /// <param name="deltaTimeInSeconds">Time in Seconds to wait between retries</param>
        /// <param name="maxTimeIntervalInSeconds">The maximum amount to wait between retries</param>
        /// <returns></returns>
        private static SqlRetryLogicBaseProvider GetExponentialBackoffProvider(int numberOfTries = 5, int deltaTimeInSeconds = 1, int maxTimeIntervalInSeconds = 20)
        {
            // Define the retry logic parameters
            var options = new SqlRetryLogicOption()
            {
                // Tries 5 times before throwing an exception
                NumberOfTries = numberOfTries,
                // Preferred gap time to delay before retry
                DeltaTime = TimeSpan.FromSeconds(deltaTimeInSeconds),
                // Maximum gap time for each delay time before retry
                MaxTimeInterval = TimeSpan.FromSeconds(maxTimeIntervalInSeconds)
            };

            return SqlConfigurableRetryFactory.CreateExponentialRetryProvider(options);
        }

        /// <summary>
        /// Sets the AppSwitch to enable retry-logic.
        /// </summary>
        private static void SetFeatureFlags()
        {
            // https://learn.microsoft.com/en-us/sql/connect/ado-net/appcontext-switches?view=sql-server-ver16#enable-configurable-retry-logic
            AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.EnableRetryLogic", true);
        }
    }
}