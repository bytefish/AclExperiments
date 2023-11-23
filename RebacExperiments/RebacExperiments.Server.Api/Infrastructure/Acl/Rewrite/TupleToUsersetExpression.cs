namespace RebacExperiments.Server.Api.Infrastructure.Acl.Rewrite
{
    /// <summary>
    ///  Computes a tupleset (§2.4.1) from the input object, fetches relation tuples matching the tupleset, and computes 
    ///  a userset from every fetched relation tuple.This flexible primitive allows our clients to express complex
    ///  policies such as "Look up the 'parent' Folder of the Document and inherit 
    ///  its 'viewers'".
    /// </summary>
    public class TupleToUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Tupleset.
        /// </summary>
        public required TuplesetExpression TuplesetExpression { get; set; }

        /// <summary>
        /// Gets or sets the Computer Userset.
        /// </summary>
        public required ComputedUsersetExpression ComputedUsersetExpression { get; set; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitTupleToUsersetExpr(this);
        }
    }
}
