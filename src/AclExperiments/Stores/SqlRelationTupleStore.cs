// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Database.Connections;
using AclExperiments.Database.Extensions;
using AclExperiments.Database.Model;
using AclExperiments.Database.Query;
using AclExperiments.Models;
using AclExperiments.Utils;
using Microsoft.Data.SqlClient.Server;
using System.Data;
using System.Data.Common;

namespace AclExperiments.Stores
{
    /// <summary>
    /// EntityFramework Core-based implementation of a <see cref="IRelationTupleStore"/>.
    /// </summary>
    public class SqlRelationTupleStore : IRelationTupleStore
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public SqlRelationTupleStore(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<List<AclRelation>> GetRelationTuplesAsync(RelationTupleQuery relationTupleQuery, CancellationToken cancellationToken)
        {
            using (var connection = await _sqlConnectionFactory.GetDbConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                var query = new SqlQuery(connection).Proc("[Identity].[usp_RelationTuple_GetRelationTuples]")
                    .Param("Namespace", relationTupleQuery.Namespace)
                    .Param("Object", relationTupleQuery.Object)
                    .Param("Relation", relationTupleQuery.Relation)
                    .Param("Subject", relationTupleQuery.Subject);

                var tuples = new List<SqlRelationTuple>();

                using (var reader = await query.ExecuteDataReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var tuple = MapToObject(reader);

                        tuples.Add(tuple);
                    }
                }

                return tuples
                    .Select(tuple => ConvertToAclRelation(tuple))
                    .ToList();
            }
        }

        public async Task<int> GetRelationTuplesRowCountAsync(AclObject @object, string relation, AclSubject subject, CancellationToken cancellationToken)
        {
            using (var connection = await _sqlConnectionFactory.GetDbConnectionAsync(cancellationToken))
            {
                var query = new SqlQuery(connection).Proc("[Identity].[usp_RelationTuple_GetRowCount]")
                    .Param("Namespace", @object.Namespace)
                    .Param("Object", @object.Id)
                    .Param("Relation", relation)
                    .Param("Subject", subject.FormatString())
                    .OutParam("RowCount", SqlDbType.Int);

                await query.ExecuteNonQueryAsync(cancellationToken);

                return query.GetOutParam<int>("RowCount");
            }
        }

        public async Task<List<AclSubjectSet>> GetSubjectSetsAsync(AclObject @object, string relation, CancellationToken cancellationToken)
        {
            using (var connection = await _sqlConnectionFactory.GetDbConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                var query = new SqlQuery(connection).Proc("[Identity].[usp_RelationTuple_GetSubjectSets]")
                    .Param("Namespace", @object.Namespace)
                    .Param("Object", @object.Id)
                    .Param("Relation", relation);

                var tuples = new List<SqlRelationTuple>();

                using (var reader = await query.ExecuteDataReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var tuple = MapToObject(reader);

                        tuples.Add(tuple);
                    }
                }

                return tuples.Select(x => AclSubjectSet.FromString(x.Subject)).ToList();
            }
        }

        public async Task AddRelationTuplesAsync(ICollection<AclRelation> aclRelations, int userId, CancellationToken cancellationToken)
        {
            var sqlRelationTuples = aclRelations
                .Select(aclRelation => ConvertToSqlRelationTuple(aclRelation, userId))
                .ToList();

            using (var connection = await _sqlConnectionFactory.GetDbConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                await new SqlQuery(connection).Proc("[Identity].[usp_RelationTuple_BulkInsert]")
                    .Tvp("RelationTuples", "[Identity].[udt_RelationTupleType]", ToSqlDataRecords(sqlRelationTuples))
                    .ExecuteNonQueryAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        public async Task RemoveRelationTuplesAsync(ICollection<AclRelation> aclRelations, int userId, CancellationToken cancellationToken)
        {
            // usp_RelationTuple_BulkDelete
            var sqlRelationTuples = aclRelations
                .Select(aclRelation => ConvertToSqlRelationTuple(aclRelation, userId))
                .ToList();

            using (var connection = await _sqlConnectionFactory.GetDbConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                await new SqlQuery(connection).Proc("[Identity].[usp_RelationTuple_BulkDelete]")
                    .Tvp("RelationTuples", "[Identity].[udt_RelationTupleType]", ToSqlDataRecords(sqlRelationTuples))
                    .ExecuteNonQueryAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private static AclRelation ConvertToAclRelation(SqlRelationTuple sqlRelationTuple)
        {
            var @object = new AclObject
            {
                Namespace = sqlRelationTuple.Namespace,
                Id = sqlRelationTuple.Object
            };

            var subject = AclSubjects.SubjectFromString(sqlRelationTuple.Subject);

            return new AclRelation
            {
                Object = @object,
                Relation = sqlRelationTuple.Relation,
                Subject = subject
            };
        }

        private static SqlRelationTuple ConvertToSqlRelationTuple(AclRelation aclRelation, int userId)
        {
            return new SqlRelationTuple
            {
                Namespace = aclRelation.Object.Namespace,
                Object = aclRelation.Object.Id,
                Relation = aclRelation.Relation,
                Subject = aclRelation.Subject.FormatString(),
                LastEditedBy = userId
            };
        }

        private static SqlRelationTuple MapToObject(DbDataReader source)
        {
            return new SqlRelationTuple
            {
                Id = source.GetInt32("RelationTupleID"),
                Namespace = source.GetString("Namespace"),
                Object = source.GetString("Object"),
                Relation = source.GetString("Relation"),
                Subject = source.GetString("Subject"),
                LastEditedBy = source.GetInt32("LastEditedBy"),
                RowVersion = source.GetByteArray("RowVersion"),
                ValidFrom = source.GetNullableDateTime("ValidFrom"),
                ValidTo = source.GetNullableDateTime("ValidTo")
            };
        }

        private static IEnumerable<SqlDataRecord> ToSqlDataRecords(IEnumerable<SqlRelationTuple> tuples)
        {
            SqlDataRecord sdr = new SqlDataRecord(
                new SqlMetaData("RelationTupleID", SqlDbType.Int),
                new SqlMetaData("Namespace", SqlDbType.NVarChar, 50),
                new SqlMetaData("Object", SqlDbType.NVarChar, 50),
                new SqlMetaData("Relation", SqlDbType.NVarChar, 50),
                new SqlMetaData("Subject", SqlDbType.NVarChar, 50),
                new SqlMetaData("RowVersion", SqlDbType.VarBinary, 8),
                new SqlMetaData("LastEditedBy", SqlDbType.Int),
                new SqlMetaData("ValidFrom", SqlDbType.DateTime2),
                new SqlMetaData("ValidTo", SqlDbType.DateTime2));

            foreach (var tuple in tuples)
            {
                sdr.SetNullableInt32(0, tuple.Id);
                sdr.SetString(1, tuple.Namespace);
                sdr.SetString(2, tuple.Object);
                sdr.SetString(3, tuple.Relation);
                sdr.SetString(4, tuple.Subject);
                sdr.SetNullableBytes(5, tuple.RowVersion);
                sdr.SetInt32(6, tuple.LastEditedBy);
                sdr.SetNullableDateTime(7, tuple.ValidFrom);
                sdr.SetNullableDateTime(8, tuple.ValidTo);

                yield return sdr;
            }
        }
    }
}