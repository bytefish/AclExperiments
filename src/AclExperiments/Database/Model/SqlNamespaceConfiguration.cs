namespace AclExperiment.CheckExpand.Database.Model
{
    /// <summary>
    /// A Namespace Configuration in the Google Zanzibar language format.
    /// </summary>
    public class SqlNamespaceConfiguration
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the Version.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        public required string Content { get; set; }
    }
}
