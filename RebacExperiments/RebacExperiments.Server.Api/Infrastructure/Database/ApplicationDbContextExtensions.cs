// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Infrastructure.Database
{
    /// <summary>
    /// Extensions on the <see cref="ApplicationDbContext"/> to allow Relationship-based ACL.
    /// </summary>
    public static class ApplicationDbContextExtensions
    {
        /// <summary>
        /// Checks if a <typeparamref name="TSubjectType"/> is authorized to access an <typeparamref name="TObjectType"/>. 
        /// </summary>
        /// <typeparam name="TObjectType">Object Type</typeparam>
        /// <typeparam name="TSubjectType">Subject Type</typeparam>
        /// <param name="context">DbContext</param>
        /// <param name="objectId">Object Key</param>
        /// <param name="relation">Relation</param>
        /// <param name="subjectId">SubjectKey</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns><see cref="true"/>, if the <typeparamref name="TSubjectType"/> is authorized; else <see cref="false"/></returns>
        public static async Task<bool> CheckObject<TObjectType, TSubjectType>(this ApplicationDbContext context, int objectId, string relation, int subjectId, CancellationToken cancellationToken)
            where TObjectType : Entity
            where TSubjectType : Entity
        {
            context.Logger.TraceMethodEntry();

            var result = await context.Database
                .SqlQuery<bool>($"SELECT [Identity].[udf_RelationTuples_Check]({typeof(TObjectType).Name}, {objectId}, {relation}, {typeof(TSubjectType).Name}, {subjectId})")
                .ToListAsync(cancellationToken);

            return result.First();
        }

        /// <summary>
        /// Checks if a <see cref="User"/> is authorized to access an <typeparamref name="TObjectType"/>. 
        /// </summary>
        /// <typeparam name="TObjectType">Object Type</typeparam>
        /// <param name="context">DbContext</param>
        /// <param name="objectId">Object Key</param>
        /// <param name="relation">Relation</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns><see cref="true"/>, if the <typeparamref name="TSubjectType"/> is authorized; else <see cref="false"/></returns>
        public static Task<bool> CheckUserObject<TObjectType>(this ApplicationDbContext context, int userId, int objectId, string relation, CancellationToken cancellationToken)
            where TObjectType : Entity
        {
            context.Logger.TraceMethodEntry();

            return CheckObject<TObjectType, User>(context, objectId, relation, userId, cancellationToken);
        }

        /// <summary>
        /// Checks if a <see cref="User"/> is authorized to access an <typeparamref name="TObjectType"/>. 
        /// </summary>
        /// <typeparam name="TObjectType">Object Type</typeparam>
        /// <param name="context">DbContext</param>
        /// <param name="objectId">Object Key</param>
        /// <param name="relation">Relation</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns><see cref="true"/>, if the <typeparamref name="TSubjectType"/> is authorized; else <see cref="false"/></returns>
        public static Task<bool> CheckUserObject<TObjectType>(this ApplicationDbContext context, int userId, TObjectType @object, string relation, CancellationToken cancellationToken)
            where TObjectType : Entity
        {
            context.Logger.TraceMethodEntry();

            return CheckObject<TObjectType, User>(context, @object.Id, relation, userId, cancellationToken);
        }

        /// <summary>
        /// Returns all <typeparamref name="TObjectType"/> for a given <typeparamref name="TSubjectType"/> and <paramref name="relation"/>.
        /// </summary>
        /// <param name="subjectId">Subject Key to resolve</param>
        /// <param name="relation">Relation between the Object and Subject</param>
        /// <returns>All <typeparamref name="TEntityType"/> the user is related to</returns>
        public static IQueryable<TObjectType> ListObjects<TObjectType, TSubjectType>(this ApplicationDbContext context, int subjectId, string relation)
            where TObjectType : Entity
            where TSubjectType : Entity
        {
            context.Logger.TraceMethodEntry();

            return
                from entity in context.Set<TObjectType>()
                join objects in context.ListObjects(typeof(TObjectType).Name, relation, typeof(TSubjectType).Name, subjectId)
                    on entity.Id equals objects.ObjectKey
                select entity;
        }

        /// <summary>
        /// Returns all <typeparamref name="TObjectType"/> for a given <typeparamref name="TSubjectType"/> and a list of Relations.
        /// </summary>
        /// <param name="subjectId">Subject Key to resolve</param>
        /// <param name="relation">Relation between the Object and Subject</param>
        /// <returns>All <typeparamref name="TEntityType"/> the user is related to</returns>
        public static IQueryable<TObjectType> ListObjects<TObjectType, TSubjectType>(this ApplicationDbContext context, int subjectId, string[] relations)
            where TObjectType : Entity
            where TSubjectType : Entity
        {
            context.Logger.TraceMethodEntry();

            return relations
                .Select(relation => ListObjects<TObjectType, TSubjectType>(context, subjectId, relation))
                .Aggregate((current, next) => current.Union(next));
        }

        /// <summary>
        /// Returns all <typeparamref name="TEntityType"/> for a given <paramref name="userId"/> and <paramref name="relation"/>.
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="relation">Relation between the User and a <typeparamref name="TEntityType"/></param>
        /// <returns>All <typeparamref name="TEntityType"/> the user is related to</returns>
        public static IQueryable<TEntityType> ListUserObjects<TEntityType>(this ApplicationDbContext context, int userId, string relation)
            where TEntityType : Entity
        {
            context.Logger.TraceMethodEntry();

            return context.ListObjects<TEntityType, User>(userId, relation);
        }

        /// <summary>
        /// Returns all <typeparamref name="TEntityType"/> for a given <paramref name="userId"/> and <paramref name="relation"/>.
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="relation">Relation between the User and a <typeparamref name="TEntityType"/></param>
        /// <returns>All <typeparamref name="TEntityType"/> the user is related to</returns>
        public static IQueryable<TEntityType> ListUserObjects<TEntityType>(this ApplicationDbContext context, int userId, string[] relations)
            where TEntityType : Entity
        {
            context.Logger.TraceMethodEntry();

            return context.ListObjects<TEntityType, User>(userId, relations);
        }

        /// <summary>
        /// Creates a Relationship between a <typeparamref name="TObjectType"/> and a <typeparamref name="TSubjectType"/>.
        /// </summary>
        /// <typeparam name="TObjectType">Type of the Object</typeparam>
        /// <typeparam name="TSubjectType">Type of the Subject</typeparam>
        /// <param name="context">DbContext</param>
        /// <param name="object">Object Entity</param>
        /// <param name="relation">Relation between Object and Subject</param>
        /// <param name="subject">Subject Entity</param>
        /// <param name="subjectRelation">Relation to the Subject</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public static async Task AddRelationshipAsync<TObjectType, TSubjectType>(this ApplicationDbContext context, TObjectType @object, string relation, TSubjectType subject, string? subjectRelation, int lastEditedBy, CancellationToken cancellationToken = default)
            where TObjectType : Entity
            where TSubjectType : Entity
        {
            context.Logger.TraceMethodEntry();

            var relationTuple = new RelationTuple
            {
                ObjectNamespace = typeof(TObjectType).Name,
                ObjectKey = @object.Id,
                ObjectRelation = relation,
                SubjectNamespace = typeof(TSubjectType).Name,
                SubjectKey = subject.Id,
                SubjectRelation = subjectRelation,
                LastEditedBy = lastEditedBy
            };

            await context.Set<RelationTuple>().AddAsync(relationTuple, cancellationToken);
        }

        /// <summary>
        /// Creates a Relationship between a <typeparamref name="TObjectType"/> and a <typeparamref name="TSubjectType"/>.
        /// </summary>
        /// <typeparam name="TObjectType">Type of the Object</typeparam>
        /// <typeparam name="TSubjectType">Type of the Subject</typeparam>
        /// <param name="context">DbContext</param>
        /// <param name="objectId">Object Entity</param>
        /// <param name="relation">Relation between Object and Subject</param>
        /// <param name="subjectId">Subject Entity</param>
        /// <param name="subjectRelation">Relation to the Subject</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public static async Task AddRelationshipAsync<TObjectType, TSubjectType>(this ApplicationDbContext context, int objectId, string relation, int subjectId, string? subjectRelation, int lastEditedBy, CancellationToken cancellationToken = default)
            where TObjectType : Entity
            where TSubjectType : Entity
        {
            context.Logger.TraceMethodEntry();

            var relationTuple = new RelationTuple
            {
                ObjectNamespace = typeof(TObjectType).Name,
                ObjectKey = objectId,
                ObjectRelation = relation,
                SubjectNamespace = typeof(TSubjectType).Name,
                SubjectKey = subjectId,
                SubjectRelation = subjectRelation,
                LastEditedBy = lastEditedBy
            };

            await context.Set<RelationTuple>().AddAsync(relationTuple, cancellationToken);
        }
    }
}