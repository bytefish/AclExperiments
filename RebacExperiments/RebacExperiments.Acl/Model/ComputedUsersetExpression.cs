namespace RebacExperiments.Acl.Model
{
    /// <summary>
    /// Computes, for the input object, a new userset. For example, this allows the userset expression for 
    /// a viewer relation to refer to the editor userset on the same object, thus offering an ACL inheritance
    /// capability between relations.
    /// </summary>
    public class ComputedUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Object,
        /// </summary>
        public string? Object { get; set; }

        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        public required string Relation { get; set; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitComputedUsersetExpr(this);
        }
    }
}
