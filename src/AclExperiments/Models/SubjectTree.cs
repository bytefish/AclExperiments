// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Expressions;

namespace AclExperiments.Models
{
    /// <summary>
    /// The expanded Subject Tree.
    /// </summary>
    public record SubjectTree
    {
        /// <summary>
        /// Gets or sets the Userset Expression for this.
        /// </summary>
        public required UsersetExpression Expression { get; set; }

        /// <summary>
        /// Gets or sets the determined Subjects.
        /// </summary>
        public HashSet<AclSubject> Result { get; set; } = [];

        /// <summary>
        /// Gets or sets the Children Trees.
        /// </summary>
        public List<SubjectTree> Children { get; set; } = new();
    }
}
