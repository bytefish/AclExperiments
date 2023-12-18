// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AclExperiments.Expressions
{
    /// <summary>
    ///  Computes a tupleset (§2.4.1) from the input object, fetches relation tuples matching the tupleset, and computes 
    ///  a userset from every fetched relation tuple.This flexible primitive allows our clients to express complex
    ///  policies such as "Look up the 'parent' Folder of the Document and inherit 
    ///  its 'viewers'".
    /// </summary>
    public record TupleToUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Tupleset.
        /// </summary>
        [JsonPropertyName("tupleset")]
        public required TuplesetExpression TuplesetExpression { get; set; }

        /// <summary>
        /// Gets or sets the Computer Userset.
        /// </summary>
        [JsonPropertyName("computed_userset")]
        public required ComputedUsersetExpression ComputedUsersetExpression { get; set; }
    }
}
