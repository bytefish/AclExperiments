// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Database.Model
{
    public class SqlRelationTuple : SqlEntity
    {
        /// <summary>
        /// Gets or sets the Object Namespace.
        /// </summary>
        public required string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the Object ID.
        /// </summary>
        public required string Object { get; set; }

        /// <summary>
        /// Gets or sets the Object Relation to a Subject.
        /// </summary>
        public required string Relation { get; set; }

        /// <summary>
        /// Gets or sets Subject Namespace.
        /// </summary>
        public required string Subject { get; set; }
    }
}
