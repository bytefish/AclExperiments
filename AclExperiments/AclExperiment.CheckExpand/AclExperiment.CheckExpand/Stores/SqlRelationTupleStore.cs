using AclExperiment.CheckExpand.Database;
using AclExperiment.CheckExpand.Database.Model;
using AclExperiment.CheckExpand.Models;
using Microsoft.EntityFrameworkCore;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AclExperiment.CheckExpand.Stores
{
    public class SqlRelationTupleStore : IRelationTupleStore
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public SqlRelationTupleStore(IDbContextFactory<ApplicationDbContext> dbContextFactory) 
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<IList<AclRelation>> GetRelationTuplesAsync(string @namespace, string? @object, string[]? relations, string? subject, CancellationToken cancellationToken)
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                IQueryable<SqlRelationTuple> queryable = context.SqlRelationTuples;

                queryable = queryable
                    .Where(x => x.Namespace == @namespace);

                if (@object != null)
                {
                    queryable = queryable
                        .Where(x => x.Object == @object);
                }

                if (relations != null)
                {
                    queryable = queryable
                        .Where(x => relations.Contains(x.Relation));
                }

                if (subject != null)
                {
                    queryable = queryable
                        .Where(x => EF.Functions.Like(x.Subject, subject));
                }

                var tuples = await queryable
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return tuples
                    .Select(tuple => ConvertToAclRelation(tuple))
                    .ToList();
            }
        }

        /// <summary>
        /// Converts from a <see cref="SqlRelationTuple"/> to a <see cref="AclRelation"/>.
        /// </summary>
        /// <param name="sqlRelationTuple">The SQL Representation of the Relation</param>
        /// <returns>The <see cref="AclRelation"/></returns>
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

        /// <summary>
        /// Returns the number of exact matches for a given <see cref="AclRelation"/>.
        /// </summary>
        /// <param name="relation">The <see cref="AclRelation"/> to query for</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns></returns>
        public async Task<int> GetRelationTuplesRowCountAsync(RelationTupleQuery query, CancellationToken cancellationToken)
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                var subject = AclSubjects.SubjectToString(query.Subject);

                int count = await context.SqlRelationTuples
                    .Where(x => x.Namespace == query.Object.Namespace)
                    .Where(x => x.Object == query.Object.Id)
                    .Where(x => query.Relations.Contains(x.Relation))
                    .Where(x => x.Subject == subject)
                    .CountAsync(cancellationToken);

                return count;
            }
        }

        public async Task<List<AclSubjectSet>> GetSubjectSetsAsync(AclObject aclObject, string[] relations, CancellationToken cancellationToken)
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                List<SqlRelationTuple> tuples = await context.SqlRelationTuples
                    .Where(x => x.Namespace == aclObject.Namespace
                        && x.Object == aclObject.Id
                        && relations.Contains(x.Relation)
                        && EF.Functions.Like("_%%:_%%#_%%", x.Subject))
                    .ToListAsync(cancellationToken);

                return tuples.Select(x => AclSubjectSet.FromString(x.Subject)).ToList();
            }
        }
    }
}
