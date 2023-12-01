// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Expressions
{
    /// <summary>
    /// The root node of the Zanzibar Configuration language. It contains the 
    /// name of the configured subject and an optional list of relations, expressed 
    /// as <see cref="RelationUsersetExpression"/>.
    /// </summary>
    public record NamespaceUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Namespace being configured.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the Relations expressed by the Namespace configuration.
        /// </summary>
        public Dictionary<string, RelationUsersetExpression> Relations { get; set; } = new();
    }
}
