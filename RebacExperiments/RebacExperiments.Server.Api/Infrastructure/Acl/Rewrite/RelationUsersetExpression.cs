namespace RebacExperiments.Server.Api.Infrastructure.Acl.Rewrite
{
    /// <summary>
    /// A Relation is expressed by its name and an optional rewrite, which is expressed as a 
    /// <see cref="UsersetExpression"/>. 
    /// </summary>
    public class RelationUsersetExpression : UsersetExpression
    {
        public required string Name { get; set; }

        public UsersetExpression? Rewrite { get; set; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitRelationUsersetExpr(this);
        }
    }
}
