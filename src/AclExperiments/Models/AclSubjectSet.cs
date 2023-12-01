// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Models
{
    /// <summary>
    /// A Subject Set.
    /// </summary>
    public record AclSubjectSet : AclSubject
    {
        /// <summary>
        /// Gets or sets the Namespace.
        /// </summary>
        public required string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the Object.
        /// </summary>
        public required string Object { get; set; }

        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        public required string Relation { get; set; }

        /// <summary>
        /// Formats the <see cref="AclSubjectSet"/> as a <see cref="string"/> in the Google Zanzibar notation.
        /// </summary>
        /// <returns>The textual SubjectSet representation</returns>
        public string FormatString()
        {
            return string.Format("{0}:{1}#{2}", Namespace, Object, Relation);
        }

        /// <summary>
        /// Parses a given <see cref="string"/> in Google Zanzibar notation to an <see cref="AclSubjectSet"/>.
        /// </summary>
        /// <param name="s">Textual representation of a Subject Set in Google Zanzibar notation</param>
        /// <returns>The <see cref="AclSubject"/> for the given text</returns>
        /// <exception cref="InvalidOperationException">Thrown, if the input string is not a valid SubjectSet</exception>
        public static AclSubjectSet FromString(string s)
        {
            var parts = s.Split("#");

            if (parts.Length != 2)
            {
                throw new InvalidOperationException("Invalid SubjectSet String");
            }

            var innerParts = parts[0].Split(":");

            if (innerParts.Length != 2)
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
