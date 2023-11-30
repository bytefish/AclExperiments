namespace AclExperiment.CheckExpand.Expressions
{
    /// <summary>
    /// Everything in the Google Zanzibar configuration language can be expressed as a 
    /// Userset Expression. It also defines a visitor for visiting all Nodes of 
    /// a <see cref="UsersetExpression"/> tree.
    /// </summary>
    public abstract record UsersetExpression
    {
        /// <summary>
        /// Visits the current tree node.
        /// </summary>
        /// <typeparam name="T">Type of the Visitor</typeparam>
        /// <param name="visitor">The Node Visitor</param>
        /// <returns>Visitor Type</returns>
        public abstract T Accept<T>(IUsersetExpressionVisitor<T> visitor);
    }
}
