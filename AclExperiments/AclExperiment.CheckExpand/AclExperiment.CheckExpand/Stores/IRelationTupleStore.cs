﻿using AclExperiment.CheckExpand.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AclExperiment.CheckExpand.Stores
{
    public interface IRelationTupleStore
    {
        /// <summary>
        /// Returns all SubjectSets for a given <see cref="AclObject"/> and Relation.
        /// </summary>
        /// <param name="aclObject">ACL Object</param>
        /// <param name="relations">Relation between Object and SubjectSets</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>An awaitable Task for SubjectSets</returns>
        Task<List<AclSubjectSet>> GetSubjectSetsAsync(AclObject aclObject, string[] relations, CancellationToken cancellationToken);


        /// <summary>
        /// For a direct check, we only need to know, if there is a matching row in the store.
        /// </summary>
        /// <param name="relation">ACL Relation between an Object and Subject</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>Number of Matching Rows for the given <see cref="AclRelation"/></returns>
        Task<int> GetRelationTuplesRowCountAsync(AclRelation relation, CancellationToken cancellationToken);

        /// <summary>
        /// Returns all RelationTuples matching the given filters.
        /// </summary>
        /// <param name="object">Object</param>
        /// <param name="relations">Relations</param>
        /// <param name="subject">Subject</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns></returns>
        Task<List<AclRelation>> GetRelationTuplesAsync(string? @object, string[]? relations, string? subject, CancellationToken cancellationToken);
    }
}