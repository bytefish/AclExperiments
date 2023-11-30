using AclExperiment.CheckExpand.Expressions;
using AclExperiment.CheckExpand.Models;
using AclExperiment.CheckExpand.Stores;
using Microsoft.Extensions.Logging;

namespace AclExperiment.CheckExpand
{
    /// <summary>
    /// The <see cref="AclService"/> implements the Expand API, Check API and ListObjects API of 
    /// the Google Zanzibar Paper.
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

        public async Task<bool> CheckAsync(string @namespace, string @object, string relation, string subject, CancellationToken cancellationToken)
        {
            // Get the latest Namespace Configuration from the Store:
            var namespaceConfiguration = await _namespaceConfigurationStore.GetLatestNamespaceConfigurationAsync(@namespace, cancellationToken);

            // Get the Rewrite for the Namespace Configuration:
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
        private UsersetExpression GetUsersetRewrite(NamespaceUsersetExpression namespaceUsersetExpression, string relation)
        {
            if (!namespaceUsersetExpression.Relations.TryGetValue(relation, out var relationUsersetExpression))
            {
                throw new InvalidOperationException($"Namespace '{namespaceUsersetExpression.Name}' has no Relation '{relation}'");
            }

            return relationUsersetExpression.Rewrite;
        }

        /// <summary>
        /// Checks a Userset 
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
            switch(rewrite)
            {
                case ThisUsersetExpression:
                case ComputedUsersetExpression:
                case TupleToUsersetExpression:
                    return await this
                        .CheckLeafNodeAsync(rewrite, @namespace, @object, relation, subject, cancellationToken)
                        .ConfigureAwait(false);
                case SetOperationUsersetExpression setOperationExpression:
                    return await this
                        .CheckSetOperationExpression(setOperationExpression, @namespace, @object, relation, subject, cancellationToken)
                        .ConfigureAwait(false);
                default:
                    throw new InvalidOperationException($"Unable to execute check for Expression '{rewrite.GetType().Name}'");
            }
        }

        private async Task<bool> CheckSetOperationExpression(SetOperationUsersetExpression setOperationExpression, string @namespace, string @object, string relation, string user, CancellationToken cancellationToken)
        {
            switch(setOperationExpression.Operation)
            {
                case SetOperationEnum.Intersect:
                    {
                        foreach(var child in setOperationExpression.Children)
                        {
                            var permitted = await this
                                .CheckLeafNodeAsync(child, @namespace, @object, relation, user, cancellationToken)
                                .ConfigureAwait(false);

                            if(!permitted)
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                case SetOperationEnum.Union:
                    {
                        foreach(var child in setOperationExpression.Children)
                        {
                            var permitted = await this
                                .CheckLeafNodeAsync(child, @namespace, @object, relation, user, cancellationToken)
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

        private async Task<bool> CheckLeafNodeAsync(UsersetExpression usersetExpression, string @namespace, string @object, string relation, string user, CancellationToken cancellationToken)
        {
            switch(usersetExpression)
            {
                case ThisUsersetExpression:
                    {
                        var aclObject = new AclObject
                        {
                            Namespace = @namespace,
                            Id = @object,
                        };

                        var query = new RelationTupleQuery
                        {
                            Object = aclObject,
                            Relations =
                            [
                                relation
                            ],
                            Subject = new AclSubjectId
                            {
                                Id = user
                            },
                        };

                        var count = await _relationTupleStore
                            .GetRelationTuplesRowCountAsync(query, cancellationToken)
                            .ConfigureAwait(false);

                        if(count > 0)
                        {
                            return true;
                        }

                        var subjestSets = await _relationTupleStore
                            .GetSubjectSetsAsync(aclObject, new[] { relation }, cancellationToken)
                            .ConfigureAwait(false);

                        foreach(var subjectSet in subjestSets)
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
                case ComputedUsersetExpression computedUsersetExpression:
                    {
                        return await this
                            .CheckAsync(@namespace, @object, computedUsersetExpression.Relation!, user, cancellationToken)
                            .ConfigureAwait(false);
                    }
                case TupleToUsersetExpression tupleToUsersetExpression:
                    {
                        var aclObject = new AclObject
                        {
                            Namespace = @namespace,
                            Id = @object
                        };

                        var subjects = await _relationTupleStore
                            .GetSubjectSetsAsync(aclObject, [ tupleToUsersetExpression.TuplesetExpression.Relation ], cancellationToken)
                            .ConfigureAwait(false);

                        if(subjects.Count == 0)
                        {
                            return false;
                        }

                        foreach( var subject in subjects)
                        {
                            relation = subject.Relation;

                            if(relation == "...")
                            {
                                relation = tupleToUsersetExpression.ComputedUsersetExpression.Relation!;

                                var permitted = await this
                                    .CheckAsync(subject.Namespace, subject.Object, relation, user, cancellationToken)
                                    .ConfigureAwait(false);

                                if(permitted)
                                {
                                    return true;
                                }
                            }
                        }

                        return false;
                    }
                default:
                    throw new InvalidOperationException($"No Check for UsersetExpression '{usersetExpression.GetType().Name}'");
            }
        }

    }
}