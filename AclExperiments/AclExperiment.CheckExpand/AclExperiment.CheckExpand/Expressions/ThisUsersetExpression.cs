namespace AclExperiment.CheckExpand.Expressions
{
    /// <summary>
    /// Returns all users from stored relation tuples for the <code>object#relation</code> pair, including 
    /// indirect ACLs referenced by usersets from the tuples.This is the default behavior when no rewrite
    /// rule is specified.
    /// </summary>
    public record ThisUsersetExpression : UsersetExpression
    {
        public override T Accept<T>(IUsersetExpressionVisitor<T> visitor)
        {
            return visitor.VisitThisUsersetExpr(this);
        }
    }
}
