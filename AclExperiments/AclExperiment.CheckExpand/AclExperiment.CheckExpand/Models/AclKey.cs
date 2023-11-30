namespace AclExperiment.CheckExpand.Models
{
    /// <summary>
    /// A Subject can be a SubjectID or a SubjectSet.
    /// </summary>
    public record AclKey
    {
        /// <summary>
        /// Gets or sets the Namespace.
        /// </summary>
        public required string? Namespace { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        public  required string? Relation { get; set; }

        /// <summary>
        /// The Subject is a SubjectID, if there is no Namespace and no Relation.
        /// </summary>
        public bool IsIdOnly => Namespace == null && Relation == null;

        /// <summary>
        /// Parse the AclKey.
        /// </summary>
        /// <param name="value">Representation in Zanzibar Notation</param>
        /// <returns>The AclKey</returns>
        public static AclKey Parse(string value)
        {
            string[] namespaceAndId2Relation = value.Split("#");

            if (namespaceAndId2Relation.Length == 1)
            {
                return new AclKey
                {
                    Namespace = null,
                    Id = value,
                    Relation = null
                };
            }

            string[] namespace2Id = namespaceAndId2Relation[0].Split(":");

            return new AclKey
            {
                Namespace = namespace2Id[0],
                Id = namespace2Id[1],
                Relation = namespaceAndId2Relation[1]
            };
        }
    }
}
