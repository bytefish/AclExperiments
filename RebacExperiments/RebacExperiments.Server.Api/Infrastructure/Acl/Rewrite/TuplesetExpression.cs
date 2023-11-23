namespace RebacExperiments.Server.Api.Infrastructure.Acl.Rewrite
{
    /// <summary>
    /// Each tupleset specifies keys of a set of relation tuples. The set can include a single tuple key, or 
    /// all tuples with a given object ID or userset in a namespace, optionally constrained by a relation 
    /// name.
    /// </summary>
    public class TuplesetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Namespace.
        /// </summary>
        public string? Namespace;

        /// <summary>
        /// Gets or sets the Object.
        /// </summary>
        public string? Object { get; set; }

        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        public required string Relation { get; set; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitTuplesetExpr(this);
        }
    }
}
