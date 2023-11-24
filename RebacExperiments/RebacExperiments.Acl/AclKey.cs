namespace RebacExperiments.Acl
{
    /// <summary>
    /// A Tuple in the Database.
    /// </summary>
    public record AclKey
    {
        /// <summary>
        /// Gets or sets the Namespace (optional).
        /// </summary>
        public string? Namespace { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        public string? Relation { get; set; }

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

        public bool IdOnly => Namespace == null && Relation == null;

        public bool Match(AclKey other)
        {
            return (Namespace == null || Namespace.Equals(other.Namespace))
                && (Id == null || Id.Equals(other.Id))
                && (Relation == null || Relation.Equals(other.Relation));
        }
    }
}
