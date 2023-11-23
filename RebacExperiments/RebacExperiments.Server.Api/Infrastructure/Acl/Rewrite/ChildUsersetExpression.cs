namespace RebacExperiments.Server.Api.Infrastructure.Acl.Rewrite
{
    /// <summary>
    /// A Leaf Node of a <see cref="SetOperationUsersetExpression"/>.
    /// </summary>
    public class ChildUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Userset Expression for this leaf node.
        /// </summary>
        public required UsersetExpression Userset { get; set; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitChildUsersetExpr(this);
        }
    }
}
