// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Database.Model;
using AclExperiments.Expressions;
using AclExperiments.Parser;
using Microsoft.Data.SqlClient.Server;
using System.Data.Common;
using System.Data;
using AclExperiments.Database.Extensions;
using AclExperiments.Database.Connections;
using AclExperiments.Database.Query;

namespace AclExperiments.Stores
{
    /// <summary>
    /// EntityFramework-core based implementation of a <see cref="INamespaceConfigurationStore"/>.
    /// </summary>
    public class SqlNamespaceConfigurationStore : INamespaceConfigurationStore
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public SqlNamespaceConfigurationStore(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<NamespaceUsersetExpression> GetLatestNamespaceConfigurationAsync(string name, CancellationToken cancellationToken)
        {
            using (var connection = await _sqlConnectionFactory.GetDbConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                var query = new SqlQuery(connection).Proc("[Identity].[usp_NamespaceConfiguration_GetLatestByName]")
                    .Param("Name", name);

                var tuples = new List<SqlNamespaceConfiguration>();

                using (var reader = await query.ExecuteDataReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var tuple = MapToObject(reader);

                        tuples.Add(tuple);
                    }
                }

                if(tuples.Count == 0)
                {
                    throw new InvalidOperationException($"No Namespace Configuration with Name '{name}' found");
                }

                return NamespaceUsersetRewriteParser.Parse(tuples[0].Content);
            }
        }

        public async Task<NamespaceUsersetExpression> GetNamespaceConfigurationAsync(string name, int version, CancellationToken cancellationToken)
        {
            using (var connection = await _sqlConnectionFactory.GetDbConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                var query = new SqlQuery(connection).Proc("[Identity].[usp_NamespaceConfiguration_GetByNameAndVersion]")
                    .Param("Name", name)
                    .Param("Version", version);

                var tuples = new List<SqlNamespaceConfiguration>();

                using (var reader = await query.ExecuteDataReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var tuple = MapToObject(reader);

                        tuples.Add(tuple);
                    }
                }

                if (tuples.Count == 0)
                {
                    throw new InvalidOperationException($"No Namespace Configuration with Name '{name}' found");
                }

                return NamespaceUsersetRewriteParser.Parse(tuples[0].Content);
            }
        }

        public async Task AddNamespaceConfigurationAsync(string name, int version, string content, CancellationToken cancellationToken)
        {
            var namespaceToInsert = new SqlNamespaceConfiguration { Name = name, Version = version, Content = content };

            using (var connection = await _sqlConnectionFactory.GetDbConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                var query = new SqlQuery(connection).Proc("[Identity].[usp_NamespaceConfiguration_BulkDelete]")
                    .Tvp("NamespaceConfigurations", "[Identity].[udt_NamespaceConfigurationType]", ToSqlDataRecords([namespaceToInsert]))
                    .ExecuteNonQueryAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        public async Task RemoveNamespaceConfigurationAsync(string name, int version, CancellationToken cancellationToken)
        {
            var namespaceToDelete = new SqlNamespaceConfiguration { Name = name, Version = version, Content = string.Empty };

            using (var connection = await _sqlConnectionFactory.GetDbConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                var query = new SqlQuery(connection).Proc("[Identity].[usp_NamespaceConfiguration_BulkDelete]")
                    .Tvp("NamespaceConfigurations", "[Identity].[udt_NamespaceConfigurationType]", ToSqlDataRecords([ namespaceToDelete ]))
                    .ExecuteNonQueryAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private static SqlNamespaceConfiguration MapToObject(DbDataReader source)
        {
            return new SqlNamespaceConfiguration
            {
                Id = source.GetInt32("NamespaceConfigurationID"),
                Name = source.GetString("Name"),
                Content = source.GetString("Content"),
                Version = source.GetInt32("Version"),
                LastEditedBy = source.GetInt32("LastEditedBy"),
                RowVersion = source.GetByteArray("RowVersion"),
                ValidFrom = source.GetNullableDateTime("ValidFrom"),
                ValidTo = source.GetNullableDateTime("ValidTo")
            };
        }

        private static IEnumerable<SqlDataRecord> ToSqlDataRecords(IEnumerable<SqlNamespaceConfiguration> tuples)
        {
            SqlDataRecord sdr = new SqlDataRecord(
                new SqlMetaData("NamespaceConfigurationID", SqlDbType.Int),
                new SqlMetaData("Name", SqlDbType.NVarChar),
                new SqlMetaData("Content", SqlDbType.NVarChar),
                new SqlMetaData("Version", SqlDbType.Int),
                new SqlMetaData("LastEditedBy", SqlDbType.Int),
                new SqlMetaData("RowVersion", SqlDbType.Binary),
                new SqlMetaData("ValidFrom", SqlDbType.DateTime2),
                new SqlMetaData("ValidTo", SqlDbType.DateTime2));

            foreach (var tuple in tuples)
            {
                sdr.SetNullableInt32(0, tuple.Id);
                sdr.SetString(1, tuple.Name);
                sdr.SetString(2, tuple.Content);
                sdr.SetInt32(3, tuple.Version);
                sdr.SetInt32(4, tuple.LastEditedBy);
                sdr.SetNullableBytes(5, tuple.RowVersion);
                sdr.SetNullableDateTime(6, tuple.ValidFrom);
                sdr.SetNullableDateTime(7, tuple.ValidTo);

                yield return sdr;
            }
        }
    }
}
