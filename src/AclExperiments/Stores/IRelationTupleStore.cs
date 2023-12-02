// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Models;

namespace AclExperiments.Stores
{
    public interface IRelationTupleStore
    {
        /// <summary>
        /// Returns all SubjectSets for a given <see cref="AclObject"/> and Relation.
        /// </summary>
        /// <param name="aclObject">Object</param>
        /// <param name="relation">Relation between Object and SubjectSets</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>An awaitable Task for SubjectSets</returns>
        Task<List<AclSubjectSet>> GetSubjectSetsAsync(AclObject aclObject, string relation, CancellationToken cancellationToken);

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
        /// <returns></returns>
        Task<List<AclRelation>> GetRelationTuplesAsync(RelationTupleQuery query, CancellationToken cancellationToken);
    }
}
