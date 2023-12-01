// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Models
{
    /// <summary>
    /// The Object of an Object to Subject Relation.
    /// </summary>
    public record AclObject
    {
        /// <summary>
        /// Gets or sets the Namespace.
        /// </summary>
        public required string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public required string Id { get; set; }
    }
}
