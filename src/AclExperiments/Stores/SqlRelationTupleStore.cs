// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Database;
using AclExperiments.Database.Model;
using AclExperiments.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;

namespace AclExperiments.Stores
{
    public class SqlRelationTupleStore : IRelationTupleStore
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public SqlRelationTupleStore(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<AclRelation>> GetRelationTuplesAsync(RelationTupleQuery relationTupleQuery, CancellationToken cancellationToken)
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                var tuples = await context.RelationTuples
                    .AsNoTracking()
                    .Where(x => (relationTupleQuery.Namespace == null || x.Namespace == relationTupleQuery.Namespace)
                        && (relationTupleQuery.Object == null || x.Object == relationTupleQuery.Object)
                        && (relationTupleQuery.Relation == null || x.Relation == relationTupleQuery.Relation)
                        && (relationTupleQuery.SubjectNamespace  == null | x.SubjectNamespace == relationTupleQuery.SubjectNamespace)
                        && (relationTupleQuery.Subject == null || x.Subject == relationTupleQuery.Subject)
                        && (relationTupleQuery.SubjectRelation == null || x.SubjectRelation == relationTupleQuery.SubjectRelation))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);

                return tuples
                    .Select(ConvertToAclRelation)
                    .ToList();
            }
        }

        public async Task<List<AclRelation>> GetRelationTuplesAsync(string@namespace, string relation,  List<AclSubject> subjects, CancellationToken cancellationToken)
        {
            // I am not sure, if should be proud or ashamed for this 🤭. 
            var parameters = subjects
                .Select(x => ExtractComponents(x))
                .Select(x => new
                {
                    SubjectNamespace = x.Namespace,
                    Subject = x.Object,
                    SubjectRelation = x.Relation,
                })
                .ToList();

            // Serialize the Tuples to JSON.
            var json = JsonSerializer.Serialize(parameters);

            // Now execute a raw SQL using the JSON String as Parameters.
            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                var tuples = context.RelationTuples
                    .FromSqlInterpolated(@$"
                        WITH QuerySubjects AS (
	                        SELECT [SubjectNamespace], [Subject], [SubjectRelation]
	                        FROM OPENJSON({json}) WITH (
		                        [SubjectNamespace] NVARCHAR(50) '$.SubjectNamespace',
		                        [Subject] NVARCHAR(50) '$.Subject',
		                        [SubjectRelation] NVARCHAR(50) '$.SubjectRelation'
	                        )
                        )
                        SELECT r.*
                        FROM [Identity].[RelationTuple] r
	                        INNER JOIN QuerySubjects q ON r.[SubjectNamespace] = q.[SubjectNamespace] 
		                        AND r.[Subject] = q.[Subject]
		                        AND ((r.[SubjectRelation] = q.[SubjectRelation]) OR (r.[SubjectRelation] IS NULL AND q.[SubjectRelation] IS NULL))
                        WHERE
	                        r.[Namespace] = {@namespace} AND r.[Relation] = {relation}")
                    .ToList();

                return tuples
                    .Select(ConvertToAclRelation)
                    .ToList();
            }

        }

        (string? Namespace, string Object, string? Relation) ExtractComponents(AclSubject subject)
        {
            switch (subject)
            {
                case AclSubjectId subjectId:
                    return (subjectId.Namespace, subjectId.Id, null);
                case AclSubjectSet subjectSet:
                    return (subjectSet.Namespace, subjectSet.Object, subjectSet.Relation);
                default:
                    throw new NotImplementedException();
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

            using(var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                int tuplesCount = await context.RelationTuples
                    .AsNoTracking()
                    .Where(x => x.Namespace == @object.Namespace
                        && x.Object == @object.Id
                        && x.Relation == relation
                        && x.Subject == subjectId.Id)
                    .CountAsync(cancellationToken)
                    .ConfigureAwait(false);

                return tuplesCount;
            }
        }

        private async Task<int> GetRelationTuplesRowCountAsync(AclObject @object, string relation, AclSubjectSet subjectSet, CancellationToken cancellationToken)
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                int tuplesCount = await context.RelationTuples
                    .AsNoTracking()
                    .Where(x => x.Namespace == @object.Namespace
                        && x.Object == @object.Id
                        && x.Relation == relation
                        && x.SubjectNamespace == subjectSet.Namespace
                        && x.Subject == subjectSet.Object
                        && x.SubjectRelation == subjectSet.Relation)
                    .CountAsync(cancellationToken)
                    .ConfigureAwait(false);

                return tuplesCount;

            }
        }

        public async Task<List<AclSubjectSet>> GetSubjectSetsForObjectAsync(AclObject @object, string relation, CancellationToken cancellationToken)
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                var tuples = await context.RelationTuples
                    .AsNoTracking()
                    .Where(x => x.Namespace == @object.Namespace
                        && x.Object == @object.Id
                        && x.Relation == relation
                        && x.SubjectNamespace != null
                        && x.Subject != null
                        && x.SubjectRelation != null)
                    .ToListAsync(cancellationToken).ConfigureAwait(false);

                return tuples.Select(GetSubjectSet).ToList();
            }
        }

        public async Task AddRelationTuplesAsync(ICollection<AclRelation> aclRelations, int userId, CancellationToken cancellationToken)
        {
            var tuples = aclRelations
                .Select(aclRelation => ConvertToSqlRelationTuple(aclRelation, userId))
                .ToList();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                // Yes, inefficient. Don't care for now ...
                await context
                    .AddRangeAsync(tuples, cancellationToken)
                    .ConfigureAwait(false);

                await context
                    .SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        public async Task RemoveRelationTuplesAsync(ICollection<AclRelation> aclRelations, int userId, CancellationToken cancellationToken)
        {
            var tuples = aclRelations
                .Select(aclRelation => ConvertToSqlRelationTuple(aclRelation, userId))
                .ToList();

            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false))
            {
                // Yes, inefficient. Don't care for now ...
                foreach(var tuple in tuples)
                {
                    await context.RelationTuples
                        .Where(t => t.Namespace == tuple.Namespace
                            && t.Object == tuple.Object
                            && t.Relation == tuple.Relation
                            && t.SubjectNamespace == tuple.SubjectNamespace
                            && t.Subject == tuple.Subject
                            && t.SubjectRelation == tuple.SubjectRelation)
                        .ExecuteDeleteAsync(cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }

        private static AclSubjectSet GetSubjectSet(SqlRelationTuple tuple)
        {
            return new AclSubjectSet
            {
                Namespace = tuple.SubjectNamespace,
                Object = tuple.Subject,
                Relation = tuple.SubjectRelation ?? "...",
            };
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
    }
}