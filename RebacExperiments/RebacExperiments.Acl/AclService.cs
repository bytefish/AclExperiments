using RebacExperiments.Acl.Model;

namespace RebacExperiments.Acl
{
    public class AclService
    {
        /// <summary>
        /// Namespace Configurations for all Entities.
        /// </summary>
        private readonly Dictionary<string, NamespaceUsersetExpression> _namespaces = new();

        /// <summary>
        /// Set of Relations.
        /// </summary>
        private readonly HashSet<AclRelation> _relations = new HashSet<AclRelation>();


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
            if (!_namespaces.ContainsKey(name))
            {
                throw new InvalidOperationException($"No Namespace Configuration '{name}'");
            }

            return _namespaces[name];
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

        public void addRelation(AclRelation relation)
        {
            _relations.Add(relation);
        }

        public void removeRelation(AclRelation relation)
        {
            _relations.Remove(relation);
        }
    }
}