namespace AclExperiment.CheckExpand.Expressions
{
    /// <summary>
    /// A Leaf Node of a <see cref="SetOperationUsersetExpression"/>.
    /// </summary>
    public record ChildUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Userset Expression for this leaf node.
        /// </summary>
        public required UsersetExpression Userset { get; set; }

        public override T Accept<T>(IUsersetExpressionVisitor<T> visitor)
        {
            return visitor.VisitChildUsersetExpr(this);
        }
    }
}
