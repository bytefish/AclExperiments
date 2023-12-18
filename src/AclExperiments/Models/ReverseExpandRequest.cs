namespace AclExperiments.Models
{
    /// <summary>
    /// Request for Reverse Expanding, so we can find all <see cref="AclObject"/> for a given <see cref="AclSubject"/>.
    /// </summary>
    public record ReverseExpandRequest
    {
        /// <summary>
        /// Gets or sets the Object Namespace.
        /// </summary>
        public required string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        public required string Relation { get; set; }

        /// <summary>
        /// Gets or sets the Subject.
        /// </summary>
        public required AclSubject Subject { get; set; }

        /// <summary>
        /// Gets or sets the Edge.
        /// </summary>
        public required RelationshipEdge? Edge { get; set; }
    }
}
