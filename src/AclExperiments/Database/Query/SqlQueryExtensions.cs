// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using System.Data;

namespace AclExperiments.Database.Query
{
    /// <summary>
    /// SQL Server Extensions for the <see cref="SqlQuery"/>.
    /// </summary>
    public static class SqlQueryExtensions
    {
        /// <summary>
        /// Sets the Command Text for the <see cref="SqlQuery"/>.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static SqlQuery Sql(this SqlQuery query, string commandText, int commandTimeOutInSeconds = 60)
        {
            var cmd = new SqlCommand()
            {
                CommandText = commandText,
                CommandType = CommandType.Text,
                CommandTimeout = commandTimeOutInSeconds,
                RetryLogicProvider = GetExponentialBackoffProvider()
            };

            return query.SetCommand(cmd);
        }

        /// <summary>
        /// Sets the Command Text for the <see cref="SqlQuery"/>.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static SqlQuery Proc(this SqlQuery query, string storedProcedureName, int commandTimeOutInSeconds = 60)
        {
            var cmd = new SqlCommand()
            {
                CommandText = storedProcedureName,
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = commandTimeOutInSeconds,
                RetryLogicProvider = GetExponentialBackoffProvider()
            };

            return query.SetCommand(cmd);
        }

        /// <summary>
        /// Add a parameter with specified value to the mapper.
        /// </summary>
        /// <param name="mapper">Mapper where the parameter will be added.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        /// <returns>Mapper object.</returns>
        public static SqlQuery Param(this SqlQuery query, string name, object? value)
        {
            if (value == null)
            {
                value = DBNull.Value;
            }

            if (query.Command is SqlCommand sqlCommand)
            {
                var p = sqlCommand.Parameters.AddWithValue(name, value);

                if (p.SqlDbType == SqlDbType.NVarChar || p.SqlDbType == SqlDbType.VarChar)
                {
                    p.Size = 100 * (value.ToString()!.Length / 100 + 1);
                }
            }

            return query;
        }

        /// <summary>
        /// Add a parameter with specified value to the mapper.
        /// </summary>
        /// <param name="mapper">Mapper where the parameter will be added.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        /// <returns>Mapper object.</returns>
        public static SqlQuery OutParam(this SqlQuery query, string name, SqlDbType sqlDbType, int? size = null)
        {
            if (query.Command is SqlCommand sqlCommand)
            {
                var p = new SqlParameter(name, sqlDbType);

                p.Direction = ParameterDirection.Output;

                if(size.HasValue)
                {
                    p.Size = size.Value;
                }
                
                sqlCommand.Parameters.Add(p);
            }

            return query;
        }

        /// <summary>
        /// Add a parameter with specified value to the mapper.
        /// </summary>
        /// <param name="mapper">Mapper where the parameter will be added.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        /// <returns>Mapper object.</returns>
        public static SqlQuery Tvp(this SqlQuery query, string name, string typeName, IEnumerable<SqlDataRecord> value)
        {
            if (query.Command is SqlCommand sqlCommand)
            {
                SqlParameter parameter = new SqlParameter();

                parameter.ParameterName = name;
                parameter.SqlDbType = SqlDbType.Structured;
                parameter.TypeName = typeName;
                parameter.Value = value;

                sqlCommand.Parameters.Add(parameter);
            }

            return query;
        }

        /// <summary>
        /// As a gentle reminder, the following list of transient errors are handled by the RetryLogic if the transient errors are <see cref="null"/>:
        /// 
        ///     1204,   // The instance of the SQL Server Database Engine cannot obtain a LOCK resource at this time. Rerun your statement when there are fewer active users. Ask the database administrator to check the lock and memory configuration for this instance, or to check for long-running transactions.
        ///     1205,   // Transaction (Process ID) was deadlocked on resources with another process and has been chosen as the deadlock victim. Rerun the transaction
        ///     1222,   // Lock request time out period exceeded.
        ///     49918,  // Cannot process request. Not enough resources to process request.
        ///     49919,  // Cannot process create or update request. Too many create or update operations in progress for subscription "%ld".
        ///     49920,  // Cannot process request. Too many operations in progress for subscription "%ld".
        ///     4060,   // Cannot open database "%.*ls" requested by the login. The login failed.
        ///     4221,   // Login to read-secondary failed due to long wait on 'HADR_DATABASE_WAIT_FOR_TRANSITION_TO_VERSIONING'. The replica is not available for login because row versions are missing for transactions that were in-flight when the replica was recycled. The issue can be resolved by rolling back or committing the active transactions on the primary replica. Occurrences of this condition can be minimized by avoiding long write transactions on the primary.
        ///     40143,  // The service has encountered an error processing your request. Please try again.
        ///     40613,  // Database '%.*ls' on server '%.*ls' is not currently available. Please retry the connection later. If the problem persists, contact customer support, and provide them the session tracing ID of '%.*ls'.
        ///     40501,  // The service is currently busy. Retry the request after 10 seconds. Incident ID: %ls. Code: %d.
        ///     40540,  // The service has encountered an error processing your request. Please try again.
        ///     40197,  // The service has encountered an error processing your request. Please try again. Error code %d.
        ///     42108,  // Can not connect to the SQL pool since it is paused. Please resume the SQL pool and try again.
        ///     42109,  // The SQL pool is warming up. Please try again.
        ///     10929,  // Resource ID: %d. The %s minimum guarantee is %d, maximum limit is %d and the current usage for the database is %d. However, the server is currently too busy to support requests greater than %d for this database. For more information, see http://go.microsoft.com/fwlink/?LinkId=267637. Otherwise, please try again later.
        ///     10928,  // Resource ID: %d. The %s limit for the database is %d and has been reached. For more information, see http://go.microsoft.com/fwlink/?LinkId=267637.
        ///     10060,  // An error has occurred while establishing a connection to the server. When connecting to SQL Server, this failure may be caused by the fact that under the default settings SQL Server does not allow remote connections. (provider: TCP Provider, error: 0 - A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond.) (Microsoft SQL Server, Error: 10060)
        ///     997,    // A connection was successfully established with the server, but then an error occurred during the login process. (provider: Named Pipes Provider, error: 0 - Overlapped I/O operation is in progress)
        ///     233     // A connection was successfully established with the server, but then an error occurred during the login process. (provider: Shared Memory Provider, error: 0 - No process is on the other end of the pipe.) (Microsoft SQL Server, Error: 233)
        /// </summary>
        private static SqlRetryLogicBaseProvider GetExponentialBackoffProvider(int numberOfTries = 5, int deltaTimeInSeconds = 1, int maxTimeIntervalInSeconds = 20, IEnumerable<int>? transientErrors = null)
        {
            // Define the retry logic parameters
            var options = new SqlRetryLogicOption()
            {
                // Tries 5 times before throwing an exception
                NumberOfTries = numberOfTries,
                // Preferred gap time to delay before retry
                DeltaTime = TimeSpan.FromSeconds(deltaTimeInSeconds),
                // Maximum gap time for each delay time before retry
                MaxTimeInterval = TimeSpan.FromSeconds(maxTimeIntervalInSeconds),
                // List of Transient Errors to handle
                TransientErrors = transientErrors
            };

            return SqlConfigurableRetryFactory.CreateExponentialRetryProvider(options);
        }
    }
}
