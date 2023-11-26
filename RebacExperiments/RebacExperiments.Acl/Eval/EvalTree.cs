using RebacExperiments.Acl.Model;

namespace RebacExperiments.Acl.Eval
{
    public class EvalTree
    {
        public UsersetExpression? Expression { get; set; }

        public List<EvalTree> Children { get; set; } = [];

        public HashSet<AclKey> Result { get; set; } = [];
    }
}
