using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AclExperiment.CheckExpand.Expressions
{
    public interface IUsersetExpressionVisitor<T>
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
}
