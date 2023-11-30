using RebacExperiments.Acl.Model;

namespace RebacExperiments.Acl.Eval
{
    /// <summary>
    /// 
    /// </summary>
    public class UsersetTree
    {
        /// <summary>
        /// Gets or sets the Userset Expression for this.
        /// </summary>
        public UsersetExpression? Expression { get; set; }

        /// <summary>
        /// Gets or sets the evaluated Subtrees.
        /// </summary>
        public List<UsersetTree> Children { get; set; } = [];

        /// <summary>
        /// Gets or sets the Relation Tuples.
        /// </summary>
        public HashSet<AclKey> Result { get; set; } = [];
    }
}
