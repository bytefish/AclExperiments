// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Expressions
{
    /// <summary>
    /// The Set Operation to apply for a <see cref="UsersetExpression"/>.
    /// </summary>
    public enum SetOperationEnum
    {
        /// <summary>
        /// Unions together the relations/permissions referenced.
        /// </summary>
        Union = 1,

        /// <summary>
        /// Intersects the set of subjects found for the relations/permissions referenced.
        /// </summary>
        Intersect = 2,

        /// <summary>
        /// Excludes the set of subjects found for the relations/permissions referenced.
        /// </summary>
        Exclude = 3,
    }
}
