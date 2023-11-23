namespace RebacExperiments.Server.Api.Infrastructure.Acl.Rewrite
{
    /// <summary>
    /// Returns all users from stored relation tuples for the <code>object#relation</code> pair, including 
    /// indirect ACLs referenced by usersets from the tuples.This is the default behavior when no rewrite
    /// rule is specified.
    /// </summary>
    public class ThisUsersetExpression : UsersetExpression
    {
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitThisUsersetExpr(this);
        }
    }
}
