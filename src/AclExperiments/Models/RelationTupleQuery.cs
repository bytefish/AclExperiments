// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Models
{
    /// <summary>
    /// A Query for Relation Tuples.
    /// </summary>
    public record RelationTupleQuery
    {
        /// <summary>
        /// Gets or sets the Namespace.
        /// </summary>
        public string? Namespace { get; set; }

        /// <summary>
        /// Gets or sets the Object.
        /// </summary>
        public string? Object { get; set; }

        /// <summary>
        /// Gets or sets the Relations.
        /// </summary>
        public string? Relation { get; set; }

        /// <summary>
        /// Gets or sets the Subject.
        /// </summary>
        public string? SubjectNamespace { get; set; }

        /// <summary>
        /// Gets or sets the Subject.
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Gets or sets the Subject.
        /// </summary>
        public string? SubjectRelation { get; set; }
    }
}
