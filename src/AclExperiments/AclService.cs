// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Expressions;
using AclExperiments.Models;
using AclExperiments.Stores;
using AclExperiments.Utils;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace AclExperiments
{
    /// <summary>
    /// The <see cref="AclService"/> implements the Google Zanzibar algorithms, such as Expand, Check and ListObjects.
    /// </summary>
    public class AclService
    {
        private readonly ILogger _logger;
        private readonly IRelationTupleStore _relationTupleStore;
        private readonly INamespaceConfigurationStore _namespaceConfigurationStore;

        public AclService(ILogger<AclService> logger, IRelationTupleStore relationTupleStore, INamespaceConfigurationStore namespaceConfigurationStore)
        {
            _logger = logger;
            _relationTupleStore = relationTupleStore;
            _namespaceConfigurationStore = namespaceConfigurationStore;
        }

        #region Check API

        public async Task<bool> CheckAsync(string @namespace, string @object, string relation, string subject, CancellationToken cancellationToken)
        {
            // Get the latest Namespace Configuration from the Store:
            var namespaceConfiguration = await _namespaceConfigurationStore
                .GetLatestNamespaceConfigurationAsync(@namespace, cancellationToken)
                .ConfigureAwait(false);

            // Get the Rewrite for the Relation from the Namespace Configuration:
            var rewrite = GetUsersetRewrite(namespaceConfiguration, relation);

            // Check Rewrite Rules for the Relation:
            return await this
                .CheckUsersetRewriteAsync(rewrite, @namespace, @object, relation, subject, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the <see cref="UsersetExpression"/> for a given Namespace Configuration and Relation.
        /// </summary>
        /// <param name="namespaceUsersetExpression">Namespace Configuration</param>
        /// <param name="relation">Relation to Check</param>
        /// <returns>The <see cref="UsersetExpression"/> for the given relation</returns>
        /// <exception cref="InvalidOperationException">Thrown, if the Relation isn't configured in the Namespace Configuration</exception>
        private static UsersetExpression GetUsersetRewrite(NamespaceUsersetExpression namespaceUsersetExpression, string relation)
        {
            if (!namespaceUsersetExpression.Relations.TryGetValue(relation, out var relationUsersetExpression))
            {
                throw new InvalidOperationException($"Namespace '{namespaceUsersetExpression.Name}' has no Relation '{relation}'");
            }

            return relationUsersetExpression.Rewrite;
        }

        /// <summary>
        /// Checks a Userset Rewrite.
        /// </summary>
        /// <param name="rewrite">Rewrite Rule for the Relation</param>
        /// <param name="namespace">Object Namespace</param>
        /// <param name="object">Object ID</param>
        /// <param name="relation">Relation name</param>
        /// <param name="subject">Subject Name</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<bool> CheckUsersetRewriteAsync(UsersetExpression rewrite, string @namespace, string @object, string relation, string subject, CancellationToken cancellationToken)
        {
            switch (rewrite)
            {
                case ThisUsersetExpression thisUsersetExpression:
                    return await this
                        .CheckThisAsync(thisUsersetExpression, @namespace, @object, relation, subject, cancellationToken)
                        .ConfigureAwait(false);
                case ChildUsersetExpression childUsersetExpression:
                    return await this
                        .CheckUsersetRewriteAsync(childUsersetExpression.Userset, @namespace, @object, relation, subject, cancellationToken)
                        .ConfigureAwait(false);
                case ComputedUsersetExpression computedUsersetExpression:
                    return await
                        CheckComputedUsersetAsync(computedUsersetExpression, @namespace, @object, subject, cancellationToken)
                        .ConfigureAwait(false);
                case TupleToUsersetExpression tupleToUsersetExpression:
                    return await
                        CheckTupleToUsersetAsync(tupleToUsersetExpression, @namespace, @object, relation, subject, cancellationToken)
                        .ConfigureAwait(false);
                case SetOperationUsersetExpression setOperationExpression:
                    return await
                        CheckSetOperationExpression(setOperationExpression, @namespace, @object, relation, subject, cancellationToken)
                        .ConfigureAwait(false);
                default:
                    throw new InvalidOperationException($"Unable to execute check for Expression '{rewrite.GetType().Name}'");
            }
        }

        private async Task<bool> CheckSetOperationExpression(SetOperationUsersetExpression setOperationExpression, string @namespace, string @object, string relation, string user, CancellationToken cancellationToken)
        {
            switch (setOperationExpression.Operation)
            {
                case SetOperationEnum.Intersect:
                    {
                        foreach (var child in setOperationExpression.Children)
                        {
                            var permitted = await this
                                .CheckUsersetRewriteAsync(child, @namespace, @object, relation, user, cancellationToken)
                                .ConfigureAwait(false);

                            if (!permitted)
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                case SetOperationEnum.Union:
                    {
                        foreach (var child in setOperationExpression.Children)
                        {
                            var permitted = await this
                                .CheckUsersetRewriteAsync(child, @namespace, @object, relation, user, cancellationToken)
                                .ConfigureAwait(false);

                            if (permitted)
                            {
                                return true;
                            }
                        }

                        return false;
                    }
                default:
                    throw new NotImplementedException($"No Implementation for Set Operator '{setOperationExpression.Operation}'");
            }
        }

        private async Task<bool> CheckThisAsync(ThisUsersetExpression thisUsersetExpression, string @namespace, string @object, string relation, string user, CancellationToken cancellationToken)
        {
            var aclObject = new AclObject
            {
                Namespace = @namespace,
                Id = @object,
            };

            var aclSubject = AclSubjects.SubjectFromString(user);

            var count = await _relationTupleStore
                .GetRelationTuplesRowCountAsync(aclObject, relation, aclSubject, cancellationToken)
                .ConfigureAwait(false);

            if (count > 0)
            {
                return true;
            }

            var subjestSets = await _relationTupleStore
                .GetSubjectSetsForObjectAsync(aclObject, relation, cancellationToken)
                .ConfigureAwait(false);

            foreach (var subjectSet in subjestSets)
            {
                var permitted = await this
                    .CheckAsync(subjectSet.Namespace, subjectSet.Object, subjectSet.Relation, user, cancellationToken)
                    .ConfigureAwait(false);

                if (permitted)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> CheckComputedUsersetAsync(ComputedUsersetExpression computedUsersetExpression, string @namespace, string @object, string user, CancellationToken cancellationToken)
        {
            if (computedUsersetExpression.Relation == null)
            {
                throw new InvalidOperationException("A Computed Userset requires a relation");
            }

            return await this
                .CheckAsync(@namespace, @object, computedUsersetExpression.Relation, user, cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<bool> CheckTupleToUsersetAsync(TupleToUsersetExpression tupleToUsersetExpression, string @namespace, string @object, string relation, string user, CancellationToken cancellationToken)
        {
            {
                var aclObject = new AclObject
                {
                    Namespace = @namespace,
                    Id = @object
                };

                var subjectSets = await _relationTupleStore
                    .GetSubjectSetsForObjectAsync(aclObject, tupleToUsersetExpression.TuplesetExpression.Relation, cancellationToken)
                    .ConfigureAwait(false);

                if (subjectSets.Count == 0)
                {
                    return false;
                }

                foreach (var subject in subjectSets)
                {
                    relation = subject.Relation;

                    if (relation == "...")
                    {
                        relation = tupleToUsersetExpression.ComputedUsersetExpression.Relation!;

                        var permitted = await this
                            .CheckAsync(subject.Namespace, subject.Object, relation, user, cancellationToken)
                            .ConfigureAwait(false);

                        if (permitted)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }


        #endregion Check API

        #region Expand API

        public async Task<SubjectTree> ExpandAsync(string @namespace, string @object, string relation, int depth, CancellationToken cancellationToken)
        {
            var namespaceConfiguration = await _namespaceConfigurationStore
                .GetLatestNamespaceConfigurationAsync(@namespace, cancellationToken)
                .ConfigureAwait(false);

            var usersetRewriteForRelation = GetUsersetRewrite(namespaceConfiguration, relation);

            var t = await this
                 .ExpandRewriteAsync(usersetRewriteForRelation, @namespace, @object, relation, depth, cancellationToken)
                 .ConfigureAwait(false);

            return new SubjectTree
            {
                Expression = namespaceConfiguration,
                Children = [t],
                Result = t.Result
            };
        }

        public async Task<SubjectTree> ExpandRewriteAsync(UsersetExpression rewrite, string @namespace, string @object, string relation, int depth, CancellationToken cancellationToken)
        {
            switch (rewrite)
            {
                case ThisUsersetExpression thisUsersetExpression:
                    return await this
                        .ExpandThisAsync(thisUsersetExpression, @namespace, @object, relation, depth, cancellationToken)
                        .ConfigureAwait(false);
                case ComputedUsersetExpression computedUsersetExpression:
                    return await this
                        .ExpandComputedUserSetAsync(computedUsersetExpression, @namespace, @object, relation, depth, cancellationToken)
                        .ConfigureAwait(false);
                case TupleToUsersetExpression tupleToUsersetExpression:
                    return await this
                        .ExpandTupleToUsersetAsync(tupleToUsersetExpression, @namespace, @object, relation, depth, cancellationToken)
                        .ConfigureAwait(false);
                case ChildUsersetExpression childUsersetExpression:
                    return await this
                        .ExpandRewriteAsync(childUsersetExpression.Userset, @namespace, @object, relation, depth, cancellationToken)
                        .ConfigureAwait(false);
                case SetOperationUsersetExpression setOperationExpression:
                    return await this
                        .ExpandSetOperationAsync(setOperationExpression, @namespace, @object, relation, depth, cancellationToken)
                        .ConfigureAwait(false);
                default:
                    throw new InvalidOperationException($"Unable to execute check for Expression '{rewrite.GetType().Name}'");
            }
        }

        public async Task<SubjectTree> ExpandSetOperationAsync(SetOperationUsersetExpression setOperationUsersetExpression, string @namespace, string @object, string relation, int depth, CancellationToken cancellationToken)
        {
            List<SubjectTree> children = [];

            // TODO This could be done in Parallel
            foreach (var child in setOperationUsersetExpression.Children)
            {
                var t = await this
                    .ExpandRewriteAsync(child, @namespace, @object, relation, depth, cancellationToken)
                    .ConfigureAwait(false);

                children.Add(t);
            }

            HashSet<AclSubject>? subjects = null;

            foreach (var child in children)
            {
                if (subjects == null)
                {
                    subjects = new HashSet<AclSubject>(child.Result);
                }
                else
                {
                    switch (setOperationUsersetExpression.Operation)
                    {
                        case SetOperationEnum.Union:
                            subjects.UnionWith(child.Result);
                            break;
                        case SetOperationEnum.Intersect:
                            subjects.IntersectWith(child.Result);
                            if (subjects.Count == 0)
                                goto eval;
                            break;
                        case SetOperationEnum.Exclude:
                            subjects.ExceptWith(child.Result);
                            if (subjects.Count == 0)
                                goto eval;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

        eval:

            return new SubjectTree
            {
                Expression = setOperationUsersetExpression,
                Result = subjects ?? [],
                Children = children
            };
        }

        public async Task<SubjectTree> ExpandThisAsync(ThisUsersetExpression expression, string @namespace, string @object, string relation, int depth, CancellationToken cancellationToken)
        {
            var query = new RelationTupleQuery
            {
                Namespace = @namespace,
                Object = @object,
                Relation = relation
            };

            var tuples = await _relationTupleStore
                .GetRelationTuplesAsync(query, cancellationToken)
                .ConfigureAwait(false);

            var children = new List<SubjectTree>();
            var result = new HashSet<AclSubject>();

            foreach (var tuple in tuples)
            {
                if (tuple.Subject is AclSubjectSet subjectSet)
                {
                    var rr = subjectSet.Relation;

                    if (rr == "...")
                    {
                        rr = relation;
                    }

                    var t = await this
                        .ExpandAsync(subjectSet.Namespace, subjectSet.Object, rr, depth - 1, cancellationToken)
                        .ConfigureAwait(false);

                    children.Add(t);
                }
                else
                {
                    var t = new SubjectTree
                    {
                        Expression = expression,
                        Result = [tuple.Subject]
                    };

                    children.Add(t);

                    result.Add(tuple.Subject);
                }
            }

            return new SubjectTree
            {
                Expression = expression,
                Result = result,
                Children = children
            };
        }

        public async Task<SubjectTree> ExpandComputedUserSetAsync(ComputedUsersetExpression computedUsersetExpression, string @namespace, string @object, string relation, int depth, CancellationToken cancellationToken)
        {
            if (computedUsersetExpression.Relation == null)
            {
                throw new InvalidOperationException("A Computed Userset requires a relation");
            }

            var subTree = await this
                .ExpandAsync(@namespace, @object, computedUsersetExpression.Relation, depth - 1, cancellationToken)
                .ConfigureAwait(false);

            return new SubjectTree
            {
                Expression = computedUsersetExpression,
                Children = [subTree],
                Result = subTree.Result
            };
        }

        public async Task<SubjectTree> ExpandTupleToUsersetAsync(TupleToUsersetExpression tupleToUsersetExpression, string @namespace, string @object, string relation, int depth, CancellationToken cancellationToken)
        {
            var rr = tupleToUsersetExpression.TuplesetExpression.Relation;

            if (rr == "...")
            {
                rr = relation;
            }

            var query = new RelationTupleQuery
            {
                Namespace = @namespace,
                Object = @object,
                Relation = rr
            };

            var tuples = await _relationTupleStore
                .GetRelationTuplesAsync(query, cancellationToken)
                .ConfigureAwait(false);

            var children = new List<SubjectTree>();

            var subjects = new HashSet<AclSubject>();

            foreach (var tuple in tuples)
            {
                if (tuple.Subject is AclSubjectSet subjectSet)
                {
                    rr = subjectSet.Relation;

                    if (rr == "...")
                    {
                        rr = relation;
                    }

                    var t = await this
                        .ExpandAsync(subjectSet.Namespace, subjectSet.Object, rr, depth - 1, cancellationToken)
                        .ConfigureAwait(false);

                    children.Add(new SubjectTree
                    {
                        Expression = new ComputedUsersetExpression
                        {
                            Namespace = @namespace,
                            Object = @object,
                            Relation = rr
                        },
                        Children = [t],
                        Result = t.Result
                    });

                    subjects.UnionWith(t.Result);
                }
                else
                {
                    var t = new SubjectTree
                    {
                        Expression = new ComputedUsersetExpression
                        {
                            Namespace = @namespace,
                            Object = @object,
                            Relation = rr,
                        },
                        Result = [tuple.Subject]
                    };

                    children.Add(t);
                    subjects.UnionWith(t.Result);
                }
            }

            return new SubjectTree
            {
                Expression = tupleToUsersetExpression,
                Children = children,
                Result = subjects
            };
        }

        #endregion Expand API

        #region Reverse Expand API

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespace">Namespace of the Object</param>
        /// <param name="relation">Relation between Object and Subject</param>
        /// <param name="subject">Subject to query for</param>
        public async Task ReverseExpandAsync(string @namespace, string relation, AclSubjectId subject, CancellationToken cancellationToken)
        {
            var namespaceConfigurations = await _namespaceConfigurationStore
                .GetAllNamespaceConfigurationsAsync(cancellationToken)
                .ConfigureAwait(false);

            var namespaceConfigurationsLookup = namespaceConfigurations.ToDictionary(x => x.Name, x => x);
        }

        private RelationshipEdge[] GetRelationshipEdges(Dictionary<string, NamespaceUsersetExpression> namespaceConfigurations, RelationReference target, RelationReference source, ConcurrentDictionary<string, byte> visited)
        {
            var key = ToObjectString(target);

            if(!visited.TryAdd(key, byte.MinValue))
            {
                return [];
            }

            var relation = GetRelation(namespaceConfigurations, target);

            return GetRelationshipEdgesWithTargetRewrite(
                namespaceConfigurations, 
                target,
                source,
                relation.Rewrite,
                visited);
        }

        private RelationshipEdge[] GetRelationshipEdgesWithTargetRewrite(Dictionary<string, NamespaceUsersetExpression> namespaceConfigurations, RelationReference target, RelationReference source, UsersetExpression rewrite, ConcurrentDictionary<string, byte> visited)
        {
            switch(rewrite)
            {
                case ThisUsersetExpression thisUsersetExpression:
                    return [];
                case ComputedUsersetExpression computedUsersetExpression:
                    return [];
                case TupleToUsersetExpression tupleToUsersetExpression:
                    return [];
                case SetOperationUsersetExpression setOperationUsersetExpression:
                    return [];
            }
        }

        private RelationUsersetExpression GetRelation(Dictionary<string, NamespaceUsersetExpression> namespaceConfigurations, RelationReference target)
        {
            if(!namespaceConfigurations.TryGetValue(target.Namespace, out var namespaceUserset)) 
            {
                throw new InvalidOperationException($"No Namespace named '{target.Namespace}' was found");
            }

            if(!namespaceUserset.Relations.TryGetValue(target.Relation, out var relationUsersetExpression))
            {
                throw new InvalidOperationException($"Namespace '{target.Namespace}' contains no Relation '{target.Relation}'");
            }

            return relationUsersetExpression;
        }

        private bool IsDirectlyRelated(Dictionary<string, NamespaceUsersetExpression> namespaceConfigurations, RelationReference target, RelationReference source)
        {
            var relation = GetRelation(namespaceConfigurations, target);

            return false;
        }

        private static string ToObjectString(RelationReference rr)
        {
            return $"{rr.Namespace}#{rr.Relation}";
        }

        #endregion Reverse Expand API
    }
}