// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace AclExperiments.Database.Query
{
    /// <summary>
    /// Provides a thin wrapper over ADO.NET.
    /// </summary>
    public class SqlQuery
    {
        /// <summary>
        /// Connection to the database.
        /// </summary>
        public DbConnection Connection { get; private set; }

        /// <summary>
        /// Connection to the database.
        /// </summary
        public DbTransaction? Transaction { get; private set; }

        /// <summary>
        /// Command to be executed.
        /// </summary>
        public DbCommand? Command { get; private set; }

        /// <summary>
        /// Creates a new <see cref="SqlQuery"/> based on the given Connection.
        /// </summary>
        /// <param name="connection">Connection to use</param>
        public SqlQuery(DbConnection connection)
                : this(connection, null)
        {
        }

        /// <summary>
        /// Creates a new <see cref="SqlQuery"/> based on the given Connection and Transaction.
        /// </summary>
        /// <param name="connection">Connection to use</param>
        /// <param name="transaction">Transaction to use</param>
        public SqlQuery(DbConnection connection, DbTransaction? transaction)
        {
            Connection = connection;
            Transaction = transaction;
        }

        /// <summary>
        /// Sets the <see cref="DbCommand"/> to execute.
        /// </summary>
        /// <param name="command"><see cref="DbCommand"/> to execute</param>
        /// <returns>SqlQuery with <see cref="DbCommand"/></returns>
        public SqlQuery SetCommand(DbCommand command)
        {
            Command = command;

            return this;
        }

        /// <summary>
        /// Sets the Command Type.
        /// </summary>
        /// <param name="commandType">Command Type</param>
        /// <returns>SqlQuery with Command Type</returns>
        public SqlQuery SetCommandType(CommandType commandType)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.CommandType = commandType;

            return this;
        }

        /// <summary>
        /// Sets the Command Timeout.
        /// </summary>
        /// <param name="commandTimeout">Command Timeout</param>
        /// <returns>SqlQuery with Command Timeout</returns>
        public SqlQuery SetCommandTimeout(TimeSpan commandTimeout)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.CommandTimeout = (int)commandTimeout.TotalSeconds;

            return this;
        }

        /// <summary>
        /// Adds a Parameter to the Query.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="type">Type</param>
        /// <param name="value">Value</param>
        /// <param name="size">Size</param>
        /// <returns><see cref="SqlQuery"/> with Parameter added</returns>
        public SqlQuery AddParameter(string name, DbType type, object? value, int size = 0)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            var parameter = Command.CreateParameter();

            parameter.ParameterName = name;
            parameter.DbType = type;
            parameter.Value = value;
            parameter.Size = size;

            if (size == 0)
            {
                bool isTextType = type == DbType.AnsiString || type == DbType.String;

                if (value != null && isTextType)
                {
                    parameter.Size = 100 * (value.ToString()!.Length / 100 + 1);
                }
            }
            else
            {
                parameter.Size = size;
            }

            Command.Parameters.Add(parameter);

            return this;
        }

        /// <summary>
        /// Executes the <see cref="SqlQuery"/> and returns a <see cref="DataSet"/> for the query results.
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="transaction">Transaction</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="DataSet"/> with the query results.</returns>
        public async Task<DataSet> ExecuteDataSetAsync(CancellationToken cancellationToken)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.Connection = Connection;
            Command.Transaction = Transaction;

            DataSet dataset = new DataSet();

            using (var reader = await Command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
                while (!reader.IsClosed)
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    dataset.Tables.Add(dataTable);
                }
            }

            return dataset;
        }

        /// <summary>
        /// Executes the <see cref="SqlQuery"/> and returns a <see cref="DataTable"/> for the query results.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="DataSet"/> with the query results.</returns>
        public async Task<DataTable> ExecuteDataTableAsync(CancellationToken cancellationToken)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.Connection = Connection;
            Command.Transaction = Transaction;

            DataTable dataTable = new DataTable();

            using (var reader = await Command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
                dataTable.Load(reader);
            }

            return dataTable;
        }

        /// <summary>
        /// Executes the <see cref="SqlQuery"/> and returns a <see cref="DataSet"/> for the query results.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="DataSet"/> with the query results.</returns>
        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.Connection = Connection;
            Command.Transaction = Transaction;

            int numberOfRowsAffected = await Command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

            return numberOfRowsAffected;
        }

        /// <summary>
        /// Executes the <see cref="SqlQuery"/> and returns a <see cref="DataSet"/> for the query results.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="DataSet"/> with the query results.</returns>
        public async Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.Connection = Connection;
            Command.Transaction = Transaction;

            object? scalarValue = await Command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

            return scalarValue;
        }

        public T? GetOutParam<T>(string name)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            if (!Command.Parameters.Contains(name))
            {
                throw new InvalidOperationException($"No Parameter with Name '{name}'");
            }

            if (Command.Parameters[name] == null)
            {
                return default;
            }

            var value = Command.Parameters[name].Value;

            if(value is not T)
            {
                throw new InvalidOperationException($"Cannot cast value '{value}' to Type '{typeof(T).Name}'");
            }

            return (T) value;
        }

        /// <summary>
        /// Executes the <see cref="SqlQuery"/> and returns a <see cref="DataSet"/> for the query results.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="DataSet"/> with the query results.</returns>
        public Task<DbDataReader> ExecuteDataReaderAsync(CancellationToken cancellationToken)
        {
            if (Command == null)
            {
                throw new InvalidOperationException("Command is not set");
            }

            Command.Connection = Connection;
            Command.Transaction = Transaction;

            return Command.ExecuteReaderAsync(cancellationToken);
        }
    }
}