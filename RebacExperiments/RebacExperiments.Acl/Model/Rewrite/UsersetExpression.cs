namespace RebacExperiments.Acl.Model.Rewrite
{
    /// <summary>
    /// Everything in the Google Zanzibar configuration language can be expressed as a 
    /// Userset Expression. It also defines a visitor for visiting all Nodes of 
    /// a <see cref="UsersetExpression"/> tree.
    /// </summary>
    public abstract class UsersetExpression
    {
        public interface Visitor<T>
        {
            T VisitChildUsersetExpr(ChildUsersetExpression expr);

            T VisitComputedUsersetExpr(ComputedUsersetExpression expr);

            T VisitNamespaceUsersetExpr(NamespaceUsersetExpression expr);

            T VisitRelationUsersetExpr(RelationUsersetExpression expr);

            T VisitSetOperationExpr(SetOperationUsersetExpression expr);

            T VisitThisUsersetExpr(ThisUsersetExpression expr);

            T VisitTuplesetExpr(TuplesetExpression expr);

            T VisitTupleToUsersetExpr(TupleToUsersetExpression expr);
        }

        /// <summary>
        /// Visits the current tree node.
        /// </summary>
        /// <typeparam name="T">Type of the Visitor</typeparam>
        /// <param name="visitor">The Node Visitor</param>
        /// <returns>Visitor Type</returns>
        public abstract T Accept<T>(Visitor<T> visitor);
    }
}
