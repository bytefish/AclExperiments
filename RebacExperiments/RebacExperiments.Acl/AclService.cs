using RebacExperiments.Acl.Model;

namespace RebacExperiments.Acl
{
    public class AclService
    {
        /// <summary>
        /// Namespace Configurations for all Entities.
        /// </summary>
        private readonly Dictionary<string, NamespaceUsersetExpression> _namespaces = [];

        /// <summary>
        /// Set of Relations.
        /// </summary>
        private readonly HashSet<AclRelation> _relations = [];


        public void AddNamespaceConfiguration(NamespaceUsersetExpression namespaceUsersetExpression)
        {
            if (_namespaces.ContainsKey(namespaceUsersetExpression.Name))
            {
                throw new InvalidOperationException($"Duplicate Namespace Configuration '{namespaceUsersetExpression.Name}'");
            }

            _namespaces.Add(namespaceUsersetExpression.Name, namespaceUsersetExpression);
        }

        public NamespaceUsersetExpression GetNamespaceConfiguration(string name)
        {
            if (!_namespaces.TryGetValue(name, out NamespaceUsersetExpression? value))
            {
                throw new InvalidOperationException($"No Namespace Configuration '{name}'");
            }

            return value;
        }

        /// <summary>
        /// Returns Relations for a given <see cref="AclKey"/>.
        /// </summary>
        /// <param name="object">Object</param>
        /// <returns></returns>
        public HashSet<AclRelation> GetRelations(AclKey @object)
        {
            return _relations
                .Where(aclRelation => @object.Match(aclRelation.Object))
                .ToHashSet();
        }

        public void AddRelation(AclRelation relation)
        {
            _relations.Add(relation);
        }

        public void RemoveRelation(AclRelation relation)
        {
            _relations.Remove(relation);
        }
    }
}