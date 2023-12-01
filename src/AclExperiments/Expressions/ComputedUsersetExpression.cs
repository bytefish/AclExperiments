namespace AclExperiment.CheckExpand.Expressions
{
    /// <summary>
    /// Computes, for the input object, a new userset. For example, this allows the userset expression for 
    /// a viewer relation to refer to the editor userset on the same object, thus offering an ACL inheritance
    /// capability between relations.
    /// </summary>
    public record ComputedUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Namespace.
        /// </summary>
        public string? Namespace { get; set; }

        /// <summary>
        /// Gets or sets the Object,
        /// </summary>
        public string? Object { get; set; }

        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        public string? Relation { get; set; }
    }
}
