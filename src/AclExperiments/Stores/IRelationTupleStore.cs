// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Models;

namespace AclExperiments.Stores
{
    public interface IRelationTupleStore
    {
        /// <summary>
        /// Get Objects by Subjects and Types.
        /// </summary>
        /// <param name="namespace">The target objects Namespace</param>
        /// <param name="relation">The target objects relation</param>
        /// <param name="subjects">A List of Subjects to query</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>List of Relations</returns>
        Task<List<AclRelation>> GetRelationsByObjectNamespaceAsync(string @namespace, string relation, List<AclSubject> subjects, CancellationToken cancellationToken);

        /// <summary>
        /// Returns all SubjectSets for a given <see cref="AclObject"/> and Relation.
        /// </summary>
        /// <param name="aclObject">Object</param>
        /// <param name="relation">Relation between Object and SubjectSets</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>An awaitable Task for SubjectSets</returns>
        Task<List<AclSubjectSet>> GetSubjectSetsByObjectAsync(AclObject aclObject, string relation, CancellationToken cancellationToken);

        /// <summary>
        /// For a direct check, we only need to know, if there is a matching row in the store.
        /// </summary>
        /// <param name="relation">ACL Relation between an Object and Subject</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>Number of Matching Rows for the given <see cref="AclRelation"/></returns>
        Task<int> GetRelationTuplesRowCountAsync(AclObject aclObject, string relation, AclSubject subject, CancellationToken cancellationToken);

        /// <summary>
        /// Returns all RelationTuples matching the given filters.
        /// </summary>
        /// <param name="query">Filter Values for the Query</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>All Relation Tuples matching the Query</returns>
        Task<List<AclRelation>> GetRelationTuplesAsync(RelationTupleQuery query, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a list of <see cref="AclRelation"/> tuples to the database.
        /// </summary>
        /// <param name="aclRelations">Relation tuples to insert</param>
        /// <param name="userId">UserID adding the tuple</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>Awaitable Task</returns>
        Task AddRelationTuplesAsync(ICollection<AclRelation> aclRelations, int userId, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a list of <see cref="AclRelation"/> tuples from the database.
        /// </summary>
        /// <param name="aclRelations">Relation Tuples to delete</param>
        /// <param name="userId">UserID removing the tuple</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>Awaitable Task</returns>
        Task RemoveRelationTuplesAsync(ICollection<AclRelation> aclRelations, int userId, CancellationToken cancellationToken);
    }
}
