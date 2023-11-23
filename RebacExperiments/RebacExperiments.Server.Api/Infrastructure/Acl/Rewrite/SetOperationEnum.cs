namespace RebacExperiments.Server.Api.Infrastructure.Acl.Rewrite
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
        Intersection = 2,

        /// <summary>
        /// Intersects the set of subjects found for the relations/permissions referenced.
        /// </summary>
        Exclusion = 3,
    }
}
