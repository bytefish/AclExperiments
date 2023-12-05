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
                    .Param("SubjectNamespace", relationTupleQuery.SubjectNamespace)
                    .Param("Subject", relationTupleQuery.Subject)
                    .Param("SubjectRelation", relationTupleQuery.SubjectRelation);

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

        public Task<int> GetRelationTuplesRowCountAsync(AclObject @object, string relation, AclSubject subject, CancellationToken cancellationToken)
        {
            switch(subject)
            {
                case AclSubjectId subjectId:
                    return GetRelationTuplesRowCountAsync(@object, relation, subjectId, cancellationToken);
                case AclSubjectSet subjectSet:
                    return GetRelationTuplesRowCountAsync(@object, relation, subjectSet, cancellationToken);
                default:
                    throw new NotImplementedException();

            }
        }

        private async Task<int> GetRelationTuplesRowCountAsync(AclObject @object, string relation, AclSubjectId subjectId, CancellationToken cancellationToken)
        {
            using (var connection = await _sqlConnectionFactory.GetDbConnectionAsync(cancellationToken))
            {
                var query = new SqlQuery(connection).Proc("[Identity].[usp_RelationTuple_GetRowCount]")
                    .Param("Namespace", @object.Namespace)
                    .Param("Object", @object.Id)
                    .Param("Relation", relation)
                    .Param("SubjectNamespace", subjectId.Namespace)
                    .Param("Subject", subjectId.Id)
                    .OutParam("RowCount", SqlDbType.Int);

                await query.ExecuteNonQueryAsync(cancellationToken);

                return query.GetOutParam<int>("RowCount");
            }
        }


        private async Task<int> GetRelationTuplesRowCountAsync(AclObject @object, string relation, AclSubjectSet subjectSet, CancellationToken cancellationToken)
        {
            using (var connection = await _sqlConnectionFactory.GetDbConnectionAsync(cancellationToken))
            {
                var query = new SqlQuery(connection).Proc("[Identity].[usp_RelationTuple_GetRowCount]")
                    .Param("Namespace", @object.Namespace)
                    .Param("Object", @object.Id)
                    .Param("Relation", relation)
                    .Param("SubjectNamespace", subjectSet.Namespace)
                    .Param("Subject", subjectSet.Object)
                    .Param("SubjectRelation", subjectSet.Relation)
                    .OutParam("RowCount", SqlDbType.Int);

                await query.ExecuteNonQueryAsync(cancellationToken);

                return query.GetOutParam<int>("RowCount");
            }
        }

        public async Task<List<AclSubjectSet>> GetSubjectSetsForObjectAsync(AclObject @object, string relation, CancellationToken cancellationToken)
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

            AclSubject? subject;

            if (sqlRelationTuple.SubjectRelation == null)
            {
                subject = new AclSubjectId
                {
                    Namespace = sqlRelationTuple.SubjectNamespace,
                    Id = sqlRelationTuple.Subject
                };
            } 
            else
            {
                subject = new AclSubjectSet
                {
                    Namespace = sqlRelationTuple.SubjectNamespace,
                    Object = sqlRelationTuple.Subject,
                    Relation = sqlRelationTuple.SubjectRelation
                };
            }

            return new AclRelation
            {
                Object = @object,
                Relation = sqlRelationTuple.Relation,
                Subject = subject
            };
        }

        private static SqlRelationTuple ConvertToSqlRelationTuple(AclRelation aclRelation, int userId)
        {
            var @object = aclRelation.Object;
            var relation = aclRelation.Relation;
            var subject = aclRelation.Subject;

            switch(subject)
            {
                case AclSubjectId subjectId:
                    return ConvertToSqlRelationTuple(@object, relation, subjectId, userId);
                case AclSubjectSet subjectSet:
                    return ConvertToSqlRelationTuple(@object, relation, subjectSet, userId);
                default:
                    throw new NotImplementedException();
            }
        }

        private static SqlRelationTuple ConvertToSqlRelationTuple(AclObject @object, string relation, AclSubjectId subjectId, int userId)
        {
            return new SqlRelationTuple
            {
                Namespace = @object.Namespace,
                Object = @object.Id,
                Relation = relation,
                SubjectNamespace = subjectId.Namespace,
                Subject = subjectId.Id,
                SubjectRelation = null,
                LastEditedBy = userId
            };
        }

        private static SqlRelationTuple ConvertToSqlRelationTuple(AclObject @object, string relation, AclSubjectSet subjectSet, int userId)
        {
            return new SqlRelationTuple
            {
                Namespace = @object.Namespace,
                Object = @object.Id,
                Relation = relation,
                SubjectNamespace = subjectSet.Namespace,
                Subject = subjectSet.Object,
                SubjectRelation = subjectSet.Relation,
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
                SubjectNamespace = source.GetString("SubjectNamespace"),
                Subject = source.GetString("Subject"),
                SubjectRelation = source.GetString("SubjectRelation"),
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