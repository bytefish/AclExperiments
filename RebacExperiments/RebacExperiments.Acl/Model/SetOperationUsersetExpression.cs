namespace RebacExperiments.Acl.Model
{
    /// <summary>
    /// Userset Expressions can be expressed as a union, intersection, ... and more 
    /// set operations, so we are able to define more complex authorization rules..
    /// </summary>
    public class SetOperationUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Set Operation, such as a Union.
        /// </summary>
        public SetOperationEnum Operation { get; set; }

        /// <summary>
        /// Gets or sets the Children.
        /// </summary>
        public required List<UsersetExpression> Children { get; set; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitSetOperationExpr(this);
        }
    }
}
