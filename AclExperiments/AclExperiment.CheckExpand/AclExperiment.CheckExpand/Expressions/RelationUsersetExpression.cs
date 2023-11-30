namespace AclExperiment.CheckExpand.Expressions
{
    /// <summary>
    /// A Relation is expressed by its name and an optional rewrite, which is expressed as a 
    /// <see cref="UsersetExpression"/>. 
    /// </summary>
    public record RelationUsersetExpression : UsersetExpression
    {
        public required string Name { get; set; }

        public UsersetExpression Rewrite { get; set; } = new ThisUsersetExpression();

        public override T Accept<T>(IUsersetExpressionVisitor<T> visitor)
        {
            return visitor.VisitRelationUsersetExpr(this);
        }
    }
}
