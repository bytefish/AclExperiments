// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Models
{
    /// <summary>
    /// Base class for Subjects, which is either a <see cref="AclSubjectId"/> or a <see cref="AclSubjectSet"/>.
    /// </summary>
    public abstract record AclSubject
    {
        /// <summary>
        /// Formats the given <see cref="AclSubject"/> as a <see cref="string"/>.
        /// </summary>
        /// <returns>Textual Representation of the <see cref="AclSubject"/></returns>
        public abstract string FormatString();
    }
}
