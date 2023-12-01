// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Models
{
    /// <summary>
    /// A Relation between an Object and a Subject (or SubjectSet).
    /// </summary>
    public record AclRelation
    {
        /// <summary>
        /// Gets or sets the Object.
        /// </summary>
        public required AclObject Object { get; set; }

        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        public required string Relation { get; set; }

        /// <summary>
        /// Gets or sets the Subject.
        /// </summary>
        public required AclSubject Subject { get; set; }
    }
}
