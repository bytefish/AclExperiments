// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Expressions;

namespace AclExperiments.Models
{
    public class TypeSystem
    {
        public Dictionary<string, NamespaceUsersetExpression> Namespaces { get; set; } = new();

        public static TypeSystem CreateTypeSystem(List<NamespaceUsersetExpression> namespaceUsersetExpressions)
        {
            return new TypeSystem
            {
                Namespaces = namespaceUsersetExpressions.ToDictionary(n => n.Name, n => n)
            };
        }
    }

    public static class TypeSystemExtensions
    {
        public static UsersetExpression GetRelation(this TypeSystem t, string @namespace, string relation)
        {
            if (!t.Namespaces.TryGetValue(@namespace, out var namespaceUsersetExpression))
            {
                throw new InvalidOperationException($"No Namespace '{@namespace}' found");
            }

            if (!namespaceUsersetExpression.Relations.TryGetValue(relation, out var relationUsersetExpression))
            {
                throw new InvalidOperationException($"No Relation '{relation}' found for Namespace '{@namespace}' found");
            }

            return relationUsersetExpression;
        }

        public static Dictionary<string, UsersetExpression> GetRelations(this TypeSystem t, string @namespace, string relation)
        {
            if (!t.Namespaces.TryGetValue(@namespace, out var namespaceUsersetExpression))
            {
                throw new InvalidOperationException($"No Namespace '{@namespace}' found");
            }

            return namespaceUsersetExpression.Relations;
        }

        public static List<DirectlyRelatedType> GetDirectlyRelatedTypes(this TypeSystem t, string @namespace, string relation)
        {
            if (!t.Namespaces.TryGetValue(@namespace, out var namespaceUsersetExpression))
            {
                throw new InvalidOperationException($"No Namespace '{@namespace}' found");
            }

            if (!namespaceUsersetExpression.Metadata.Relations.TryGetValue(relation, out var metadataRelationExpression))
            {
                return [];
            }

            return metadataRelationExpression.DirectlyRelatedTypes;

        }

        public static bool IsDirectlyRelated(this TypeSystem t, RelationReference target, RelationReference source)
        {
            var directlyRelatedTypes = t.GetDirectlyRelatedTypes(target.Namespace, target.Relation!);

            foreach (var directlyRelatedType in directlyRelatedTypes)
            {
                if (source.Namespace == directlyRelatedType.Namespace)
                {
                    // type with no relation
                    if (string.IsNullOrWhiteSpace(directlyRelatedType.Relation) && string.IsNullOrWhiteSpace(source.Relation))
                    {
                        return true;
                    }

                    // TODO Are Wildcards useful?

                    // type with relation
                    if (!string.IsNullOrWhiteSpace(directlyRelatedType.Relation) && !string.IsNullOrWhiteSpace(source.Relation))
                    {
                        if (directlyRelatedType.Relation == source.Relation)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
