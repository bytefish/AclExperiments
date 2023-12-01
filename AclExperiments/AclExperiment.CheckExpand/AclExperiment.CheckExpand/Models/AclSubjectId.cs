namespace AclExperiment.CheckExpand.Models
{
    /// <summary>
    /// A Subject ID.
    /// </summary>
    public record AclSubjectId : AclSubject
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public required string Id { get; set; }

        public static AclSubjectId FromString(string s)
        {
            return new AclSubjectId { Id = s };
        }

        public string FormatString()
        {
            return Id;
        }
    }
}
