// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Expressions;
using AclExperiments.Models;
using AclExperiments.Stores;
using AclExperiments.Utils;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;

namespace AclExperiments
{
    /// <summary>
    /// The <see cref="AclService"/> implements the Google Zanzibar algorithms, such as Expand, Check and ListObjects.
    /// </summary>
    public class AclService
    {
        private readonly ILogger _logger;
        private readonly IRelationTupleStore _relationTupleStore;
        private readonly IAuthorizationModelStore _authorizationModelStore;

        public AclService(ILogger<AclService> logger, IRelationTupleStore relationTupleStore, IAuthorizationModelStore authorizationModelStore)
        {
            _logger = logger;
            _relationTupleStore = relationTupleStore;
            _authorizationModelStore = authorizationModelStore;
        }

        #region Check API

        public async Task<bool> CheckAsync(TypeSystem typeSystem, string @namespace, string @object, string relation, string subject, CancellationToken cancellationToken)
        {
            // Get the latest Namespace Configuration from the Store:
            var rewrite = typeSystem.GetRelation(@namespace, relation);

            // Check Rewrite Rules for the Relation:
            return await this
                .CheckUsersetRewriteAsync(typeSystem, rewrite, @namespace, @object, relation, subject, cancellationToken)
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
            if (!namespaceUsersetExpression.Relations.TryGetValue(relation, out var rewrite))
            {
                throw new InvalidOperationException($"Namespace '{namespaceUsersetExpression.Name}' has no Relation '{relation}'");
            }

            return rewrite;
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
        public async Task<bool> CheckUsersetRewriteAsync(TypeSystem typeSystem, UsersetExpression rewrite, string @namespace, string @object, string relation, string subject, CancellationToken cancellationToken)
        {
            switch (rewrite)
            {
                case ThisUsersetExpression thisUsersetExpression:
                    return await this
                        .CheckThisAsync(typeSystem, thisUsersetExpression, @namespace, @object, relation, subject, cancellationToken)
                        .ConfigureAwait(false);
                case ChildUsersetExpression childUsersetExpression:
                    return await this
                        .CheckUsersetRewriteAsync(typeSystem, childUsersetExpression.Userset, @namespace, @object, relation, subject, cancellationToken)
                        .ConfigureAwait(false);
                case ComputedUsersetExpression computedUsersetExpression:
                    return await
                        CheckComputedUsersetAsync(typeSystem, computedUsersetExpression, @namespace, @object, subject, cancellationToken)
                        .ConfigureAwait(false);
                case TupleToUsersetExpression tupleToUsersetExpression:
                    return await
                        CheckTupleToUsersetAsync(typeSystem, tupleToUsersetExpression, @namespace, @object, relation, subject, cancellationToken)
                        .ConfigureAwait(false);
                case SetOperationUsersetExpression setOperationExpression:
                    return await
                        CheckSetOperationExpression(typeSystem, setOperationExpression, @namespace, @object, relation, subject, cancellationToken)
                        .ConfigureAwait(false);
                default:
                    throw new InvalidOperationException($"Unable to execute check for Expression '{rewrite.GetType().Name}'");
            }
        }

        private async Task<bool> CheckSetOperationExpression(TypeSystem typeSystem, SetOperationUsersetExpression setOperationExpression, string @namespace, string @object, string relation, string user, CancellationToken cancellationToken)
        {
            switch (setOperationExpression.Operation)
            {
                case SetOperationEnum.Intersect:
                    {
                        foreach (var child in setOperationExpression.Children)
                        {
                            var permitted = await this
                                .CheckUsersetRewriteAsync(typeSystem, child, @namespace, @object, relation, user, cancellationToken)
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
                                .CheckUsersetRewriteAsync(typeSystem, child, @namespace, @object, relation, user, cancellationToken)
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

        private async Task<bool> CheckThisAsync(TypeSystem typeSystem, ThisUsersetExpression thisUsersetExpression, string @namespace, string @object, string relation, string user, CancellationToken cancellationToken)
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
                    .CheckAsync(typeSystem, subjectSet.Namespace, subjectSet.Object, subjectSet.Relation, user, cancellationToken)
                    .ConfigureAwait(false);

                if (permitted)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> CheckComputedUsersetAsync(TypeSystem typeSystem, ComputedUsersetExpression computedUsersetExpression, string @namespace, string @object, string user, CancellationToken cancellationToken)
        {
            if (computedUsersetExpression.Relation == null)
            {
                throw new InvalidOperationException("A Computed Userset requires a relation");
            }

            return await this
                .CheckAsync(typeSystem, @namespace, @object, computedUsersetExpression.Relation, user, cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<bool> CheckTupleToUsersetAsync(TypeSystem typeSystem, TupleToUsersetExpression tupleToUsersetExpression, string @namespace, string @object, string relation, string user, CancellationToken cancellationToken)
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
                            .CheckAsync(typeSystem, subject.Namespace, subject.Object, relation, user, cancellationToken)
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

        public async Task<SubjectTree> ExpandAsync(TypeSystem typeSystem, string @namespace, string @object, string relation, int depth, CancellationToken cancellationToken)
        {
            var rewrite = typeSystem.GetRelation(@namespace, relation);

            var t = await this
                 .ExpandRewriteAsync(typeSystem, rewrite, @namespace, @object, relation, depth, cancellationToken)
                 .ConfigureAwait(false);

            return new SubjectTree
            {
                Expression = typeSystem.Namespaces[@namespace],
                Children = [t],
                Result = t.Result
            };
        }

        public async Task<SubjectTree> ExpandRewriteAsync(TypeSystem typeSystem, UsersetExpression rewrite, string @namespace, string @object, string relation, int depth, CancellationToken cancellationToken)
        {
            switch (rewrite)
            {
                case ThisUsersetExpression thisUsersetExpression:
                    return await this
                        .ExpandThisAsync(typeSystem, thisUsersetExpression, @namespace, @object, relation, depth, cancellationToken)
                        .ConfigureAwait(false);
                case ComputedUsersetExpression computedUsersetExpression:
                    return await this
                        .ExpandComputedUserSetAsync(typeSystem, computedUsersetExpression, @namespace, @object, relation, depth, cancellationToken)
                        .ConfigureAwait(false);
                case TupleToUsersetExpression tupleToUsersetExpression:
                    return await this
                        .ExpandTupleToUsersetAsync(typeSystem, tupleToUsersetExpression, @namespace, @object, relation, depth, cancellationToken)
                        .ConfigureAwait(false);
                case ChildUsersetExpression childUsersetExpression:
                    return await this
                        .ExpandRewriteAsync(typeSystem, childUsersetExpression.Userset, @namespace, @object, relation, depth, cancellationToken)
                        .ConfigureAwait(false);
                case SetOperationUsersetExpression setOperationExpression:
                    return await this
                        .ExpandSetOperationAsync(typeSystem, setOperationExpression, @namespace, @object, relation, depth, cancellationToken)
                        .ConfigureAwait(false);
                default:
                    throw new InvalidOperationException($"Unable to execute check for Expression '{rewrite.GetType().Name}'");
            }
        }

        public async Task<SubjectTree> ExpandSetOperationAsync(TypeSystem typeSystem, SetOperationUsersetExpression setOperationUsersetExpression, string @namespace, string @object, string relation, int depth, CancellationToken cancellationToken)
        {
            List<SubjectTree> children = [];

            // TODO This could be done in Parallel
            foreach (var child in setOperationUsersetExpression.Children)
            {
                var t = await this
                    .ExpandRewriteAsync(typeSystem, child, @namespace, @object, relation, depth, cancellationToken)
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

        public async Task<SubjectTree> ExpandThisAsync(TypeSystem typeSystem, ThisUsersetExpression expression, string @namespace, string @object, string relation, int depth, CancellationToken cancellationToken)
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
                        .ExpandAsync(typeSystem, subjectSet.Namespace, subjectSet.Object, rr, depth - 1, cancellationToken)
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

        public async Task<SubjectTree> ExpandComputedUserSetAsync(TypeSystem typeSystem, ComputedUsersetExpression computedUsersetExpression, string @namespace, string @object, string relation, int depth, CancellationToken cancellationToken)
        {
            if (computedUsersetExpression.Relation == null)
            {
                throw new InvalidOperationException("A Computed Userset requires a relation");
            }

            var subTree = await this
                .ExpandAsync(typeSystem, @namespace, @object, computedUsersetExpression.Relation, depth - 1, cancellationToken)
                .ConfigureAwait(false);

            return new SubjectTree
            {
                Expression = computedUsersetExpression,
                Children = [subTree],
                Result = subTree.Result
            };
        }

        public async Task<SubjectTree> ExpandTupleToUsersetAsync(TypeSystem typeSystem, TupleToUsersetExpression tupleToUsersetExpression, string @namespace, string @object, string relation, int depth, CancellationToken cancellationToken)
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
                        .ExpandAsync(typeSystem, subjectSet.Namespace, subjectSet.Object, rr, depth - 1, cancellationToken)
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
        /// <param name="sourceNamespace">Namespace of the Object</param>
        /// <param name="sourceRelation">Relation between Object and Subject</param>
        /// <param name="subject">Subject to query for</param>
        public async Task<List<RelationshipEdge>> ReverseExpandAsync(TypeSystem typeSystem, string targetNamespace, string targetRelation, string sourceNamespace, string? sourceRelation, CancellationToken cancellationToken)
        {
            var target = new RelationReference
            {
                Namespace = targetNamespace,
                Relation = targetRelation
            };

            var source = new RelationReference
            {
                Namespace = sourceNamespace,
                Relation = sourceRelation
            };

            var visited = new ConcurrentDictionary<string, byte>();

            return GetRelationshipEdges(typeSystem, target, source, visited);
        }

        private List<RelationshipEdge> GetRelationshipEdges(TypeSystem typeSystem, RelationReference target, RelationReference source, ConcurrentDictionary<string, byte> visited)
        {
            var key = ToObjectString(target);

            if (!visited.TryAdd(key, byte.MinValue))
            {
                return [];
            }

            var rewrite = typeSystem.GetRelation(target.Namespace, target.Relation!);

            return GetRelationshipEdgesWithTargetRewrite(
                typeSystem,
                target,
                source,
                rewrite,
                visited);
        }

        private List<RelationshipEdge> GetRelationshipEdgesWithTargetRewrite(TypeSystem typeSystem, RelationReference target, RelationReference source, UsersetExpression rewrite, ConcurrentDictionary<string, byte> visited)
        {
            switch (rewrite)
            {
                case ThisUsersetExpression thisUsersetExpression:
                    return GetRelationshipEdgesWithTargetRewrite_This(typeSystem, target, source, visited);
                case ComputedUsersetExpression computedUsersetExpression:
                    return GetRelationshipEdgesWithTargetRewrite_ComputedUserset(typeSystem, target, source, computedUsersetExpression, visited); ;
                case TupleToUsersetExpression tupleToUsersetExpression:
                    return GetRelationshipEdgesWithTargetRewrite_TupleToUserset(typeSystem, target, source, tupleToUsersetExpression, visited); ;
                case SetOperationUsersetExpression setOperationUsersetExpression:
                    return GetRelationshipEdgesWithTargetRewrite_SetOperation(typeSystem, target, source, setOperationUsersetExpression, visited); ;
                default:
                    throw new NotImplementedException();
            }
        }

        private List<RelationshipEdge> GetRelationshipEdgesWithTargetRewrite_This(TypeSystem typeSystem, RelationReference target, RelationReference source, ConcurrentDictionary<string, byte> visited)
        {
            var res = new List<RelationshipEdge>();

            bool isDirectlyRelated = typeSystem.IsDirectlyRelated(target, source);

            if (isDirectlyRelated)
            {
                var edge = new RelationshipEdge
                {
                    RelationEdgeType = RelationEdgeTypeEnum.DirectEdge,
                    TargetReference = target,
                    TargetIsIntersectionOrExclusion = false
                };

                res.Add(edge);
            }

            var directlyRelatedTypes = typeSystem.GetDirectlyRelatedTypes(target.Namespace, target.Relation!);

            foreach (var directlyRelatedType in directlyRelatedTypes)
            {
                if (directlyRelatedType.Relation != null)
                {
                    var directRelationReference = new RelationReference
                    {
                        Namespace = directlyRelatedType.Namespace,
                        Relation = directlyRelatedType.Relation
                    };

                    var edges = GetRelationshipEdges(typeSystem, directRelationReference, source, visited);

                    res.AddRange(edges);
                }
            }

            return res;
        }

        private List<RelationshipEdge> GetRelationshipEdgesWithTargetRewrite_ComputedUserset(TypeSystem typeSystem, RelationReference target, RelationReference source, ComputedUsersetExpression computedUsersetExpression, ConcurrentDictionary<string, byte> visited)
        {
            var res = new List<RelationshipEdge>();

            var sourceMatchesRewritten = target.Namespace == source.Namespace && computedUsersetExpression.Relation == source.Relation;

            if (sourceMatchesRewritten)
            {
                var targetReference = new RelationReference
                {
                    Namespace = target.Namespace,
                    Relation = target.Relation
                };

                var computedRelationshipEdge = new RelationshipEdge
                {
                    RelationEdgeType = RelationEdgeTypeEnum.ComputedUserset,
                    TargetReference = targetReference,
                    TargetIsIntersectionOrExclusion = false,
                };

                res.Add(computedRelationshipEdge);
            }

            if (computedUsersetExpression.Relation != null)
            {
                var computedTargetReference = new RelationReference { Namespace = target.Namespace, Relation = computedUsersetExpression.Relation };

                var computedTargetEdges = GetRelationshipEdges(typeSystem, computedTargetReference, source, visited);

                res.AddRange(computedTargetEdges);
            }

            return res;
        }

        private List<RelationshipEdge> GetRelationshipEdgesWithTargetRewrite_TupleToUserset(TypeSystem typeSystem, RelationReference target, RelationReference source, TupleToUsersetExpression tupleToUsersetExpression, ConcurrentDictionary<string, byte> visited)
        {
            var res = new List<RelationshipEdge>();

            var directlyRelatedTypes = typeSystem.GetDirectlyRelatedTypes(target.Namespace, tupleToUsersetExpression.TuplesetExpression.Relation);

            foreach (var directlyRelatedType in directlyRelatedTypes)
            {
                if (directlyRelatedType.Namespace == source.Namespace && tupleToUsersetExpression.ComputedUsersetExpression.Relation == source.Relation)
                {
                    // TODO Intersections...
                    var targetRelationReference = new RelationReference
                    {
                        Namespace = target.Namespace,
                        Relation = target.Relation
                    };

                    var targetIsIntersectionOrExclusion = false;

                    var targetRelationshipEdge = new RelationshipEdge
                    {
                        RelationEdgeType = RelationEdgeTypeEnum.TupleToUserset,
                        TargetReference = targetRelationReference,
                        TuplesetRelation = tupleToUsersetExpression.TuplesetExpression.Relation,
                        TargetIsIntersectionOrExclusion = targetIsIntersectionOrExclusion
                    };

                    res.Add(targetRelationshipEdge);
                }

                var directlyRelatedTypeReference = new RelationReference
                {
                    Namespace = directlyRelatedType.Namespace,
                    Relation = tupleToUsersetExpression.ComputedUsersetExpression.Relation
                };

                var subResults = GetRelationshipEdges(typeSystem, directlyRelatedTypeReference, source, visited);

                res.AddRange(subResults);
            }

            return res;
        }

        private List<RelationshipEdge> GetRelationshipEdgesWithTargetRewrite_SetOperation(TypeSystem typeSystem, RelationReference target, RelationReference source, SetOperationUsersetExpression setOperationUsersetExpression, ConcurrentDictionary<string, byte> visited)
        {
            var res = new List<RelationshipEdge>();

            switch (setOperationUsersetExpression.Operation)
            {
                case SetOperationEnum.Union:

                    foreach (var child in setOperationUsersetExpression.Children)
                    {
                        var childResults = GetRelationshipEdgesWithTargetRewrite(typeSystem, target, source, child, visited);

                        res.AddRange(childResults);
                    }
                    break;

                default:
                    throw new NotImplementedException($"No Implementation for Set Operator '{setOperationUsersetExpression.Operation}'");
            }

            return res;
        }

        private static string ToObjectString(RelationReference rr)
        {
            return $"{rr.Namespace}#{rr.Relation}";
        }

        #endregion Reverse Expand API
    }
}