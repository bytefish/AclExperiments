
namespace AclExperiment.CheckExpand.Models
{

    public record AclObject
    {
        public required string Namespace { get; set; }

        public required string Id { get; set; }
    }

    /// <summary>
    /// Base class for Subjects, which is either a <see cref="AclSubjectId"/> or a <see cref="AclSubjectSet"/>.
    /// </summary>
    public abstract record AclSubject
    {

    }

    ///
    public static class AclSubjects
    {
        public static AclSubject SubjectFromString(string s)
        {
            if (s.Contains('#'))
            {
                return AclSubjectSet.FromString(s);
            }

            return AclSubjectId.FromString(s);
        }

        public static string SubjectToString(AclSubject s)
        {
            switch(s)
            {
                case AclSubjectId subjectId:
                    return subjectId.FormatString();
                case AclSubjectSet subjectSet:
                    return subjectSet.FormatString();
                default:
                    throw new InvalidOperationException($"Cannot format Subject Type '{s.GetType().Name}'");
            }


        }
    }

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

    /// <summary>
    /// A Relation between an Object and a Subject (or SubjectSet).
    /// </summary>
    public class AclRelation
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
