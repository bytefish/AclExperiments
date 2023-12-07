// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace AclExperiments.Expressions
{
    /// <summary>
    /// Userset Expressions can be expressed as a union, intersection, ... and more 
    /// set operations, so we are able to define more complex authorization rules..
    /// </summary>
    public record SetOperationUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Set Operation, such as a Union.
        /// </summary>
        [JsonPropertyName("operation")]
        public SetOperationEnum Operation { get; set; }

        /// <summary>
        /// Gets or sets the Children.
        /// </summary>
        [JsonPropertyName("children")]
        public required List<UsersetExpression> Children { get; set; }
    }
}
