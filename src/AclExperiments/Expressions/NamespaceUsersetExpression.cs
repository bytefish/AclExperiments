// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

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
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("version")]
        public required int Version { get; set; }

        /// <summary>
        /// Gets or sets the Metadata for the Namespace.
        /// </summary>
        [JsonPropertyName("metadata")]
        public MetadataExpression Metadata { get; set; } = new MetadataExpression();

        /// <summary>
        /// Gets or sets the Relations.
        /// </summary>
        [JsonPropertyName("relations")]
        public Dictionary<string, UsersetExpression> Relations { get; set; } = new();
    }
}
