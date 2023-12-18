// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace AclExperiments.Expressions
{
    /// <summary>
    /// Each tupleset specifies keys of a set of relation tuples. The set can include a single tuple key, or 
    /// all tuples with a given object ID or userset in a namespace, optionally constrained by a relation 
    /// name.
    /// </summary>
    public record TuplesetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Namespace.
        /// </summary>
        [JsonPropertyName("namespace")]
        public string? Namespace;

        /// <summary>
        /// Gets or sets the Object.
        /// </summary>
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        [JsonPropertyName("relation")]
        public required string Relation { get; set; }
    }
}
