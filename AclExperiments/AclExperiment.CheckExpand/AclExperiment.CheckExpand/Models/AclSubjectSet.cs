namespace AclExperiment.CheckExpand.Models
{
    /// <summary>
    /// A Subject Set.
    /// </summary>
    public record AclSubjectSet : AclSubject
    {
        public required string Namespace { get; set; }
        
        public required string Object { get; set; }
        
        public required string Relation { get; set; }

        public string FormatString()
        {
            return string.Format("{0}:{1}#{2}", Namespace, Object, Relation);
        }

        public static AclSubjectSet FromString(string s)
        {
            var parts = s.Split("#");

            if(parts.Length != 2) 
            {
                throw new InvalidOperationException("Invalid SubjectSet String");
            }

            var innerParts = parts[0].Split(":");

            if(innerParts.Length != 2) 
            {
                throw new InvalidOperationException("Invalid SubjectSet String");
            }

            return new AclSubjectSet
            {
                Namespace = innerParts[0],
                Object = innerParts[1],
                Relation = innerParts[2]
            };
        }
    }
}
