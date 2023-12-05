// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Models
{
    /// <summary>
    /// A Subject ID.
    /// </summary>
    public record AclSubjectId : AclSubject
    {
        /// <summary>
        /// Gets or sets the Namespace.
        /// </summary>
        public required string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public required string Id { get; set; }

        public static AclSubjectId FromString(string s)
        {
            var parts = s.Split(':');

            if(parts.Length != 2 ) 
            {
                throw new InvalidOperationException($"'{s}' is not a valid subject id. Expected a Namespace and SubjectId, such as 'user:user_id'");
            }

            return new AclSubjectId 
            {
                Namespace = parts[0],
                Id = parts[1]
            };
        }

        public override string FormatString()
        {
            return $"{Namespace}:{Id}";
        }
    }
}
