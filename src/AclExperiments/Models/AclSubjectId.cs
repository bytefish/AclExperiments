// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Models
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

        public override string FormatString()
        {
            return Id;
        }
    }
}
