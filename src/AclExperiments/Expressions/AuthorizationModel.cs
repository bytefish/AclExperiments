// Licensed under the MIT license. See LICENSE file in the project root for full license information.


// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace AclExperiments.Expressions
{
    /// <summary>
    /// The <see cref="AuthorizationModel"/>, which contains all Namespace Configurations required 
    /// for Relation-based Authorization.
    /// </summary>
    public class AuthorizationModel
    {
        /// <summary>
        /// Gets or sets the Model ID.
        /// </summary>
        [JsonPropertyName("modelKey")]
        public required string ModelKey { get; set; }

        /// <summary>
        /// Gets or sets the Model Name.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the Model Description.
        /// </summary>
        [JsonPropertyName("description")]
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the Namespaces.
        /// </summary>
        [JsonPropertyName("namespaces")]
        public List<NamespaceUsersetExpression> Namespaces { get; set; } = new();

    }
}
