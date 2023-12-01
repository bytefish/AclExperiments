namespace AclExperiment.CheckExpand.Models
{
    /// <summary>
    /// A Query for a 
    /// </summary>
    public record RelationTupleQuery
    {
        /// <summary>
        /// Gets or sets the Object.
        /// </summary>
        public required AclObject Object { get; set; }

        /// <summary>
        /// Gets or sets the Relations.
        /// </summary>
        public required string[] Relations { get; set; }

        /// <summary>
        /// Gets or sets the Subject.
        /// </summary>
        public required AclSubject Subject { get; set; }
    }
}
