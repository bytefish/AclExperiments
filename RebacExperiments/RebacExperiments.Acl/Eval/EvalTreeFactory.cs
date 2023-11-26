using RebacExperiments.Acl.Model;

namespace RebacExperiments.Acl.Eval
{
    public class EvalTreeFactory
    {
        public static EvalTree Evaluate(AclService service, AclKey @object)
        {
            return new Builder(service, @object).Eval();
        }

        private class Builder : UsersetExpression.Visitor<EvalTree>
        {
            private readonly AclService _service;
            private readonly AclKey _object;

            public Builder(AclService service, AclKey @object)
            {
                _service = service;
                _object = @object;
            }

            public EvalTree Eval()
            {
                // TODO Fix Nullable warning
                NamespaceUsersetExpression namespaceConfig = _service.GetNamespaceConfiguration(_object.Namespace!);

                return VisitNamespaceUsersetExpr(namespaceConfig);
            }


            public EvalTree VisitChildUsersetExpr(ChildUsersetExpression expr)
            {
                EvalTree eval = expr.Userset.Accept(this);

                Stack<AclRelation> relations = new();

                foreach (var relation in eval.Result
                    .SelectMany(x => _service.GetRelations(x))
                    .ToList())
                {
                    relations.Push(relation);
                }

                HashSet<AclKey> result = [];

                while (relations.Count != 0)
                {
                    AclKey user = relations.Pop().User;

                    if (user.IdOnly)
                    {
                        result.Add(user);
                    }
                    else
                    {
                        foreach (var relation in _service.GetRelations(user))
                        {
                            relations.Push(relation);
                        }
                    }
                }

                return new EvalTree
                {
                    Expression = expr,
                    Children = [eval],
                    Result = result
                };
            }

            public EvalTree VisitComputedUsersetExpr(ComputedUsersetExpression expr)
            {
                throw new NotImplementedException();
            }

            public EvalTree VisitNamespaceUsersetExpr(NamespaceUsersetExpression expr)
            {
                if (!expr.Relations.TryGetValue(_object.Relation, out RelationUsersetExpression? r))
                {
                    throw new InvalidOperationException($"The Namespace UsersetRewrite does not contain Relation '{_object.Relation}'");
                }

                return VisitRelationUsersetExpr(r);
            }

            public EvalTree VisitRelationUsersetExpr(RelationUsersetExpression expr)
            {
                return expr.Rewrite!.Accept(this);
            }

            public EvalTree VisitSetOperationExpr(SetOperationUsersetExpression expr)
            {
                List<EvalTree> children = expr.Children
                    .Select(x => x.Accept(this))
                    .ToList();

                HashSet<AclKey>? result = [];

            loop:
                foreach (var child in children)
                {
                    if (result == null)
                    {
                        result = new HashSet<AclKey>(child.Result);
                    }
                    else
                    {
                        switch (expr.Operation)
                        {
                            case SetOperationEnum.Union:
                                result.UnionWith(child.Result);
                                break;
                            case SetOperationEnum.Intersect:
                                result.IntersectWith(child.Result);
                                if (result.Count == 0)
                                    goto loop;
                                break;
                            case SetOperationEnum.Exclude:
                                result.ExceptWith(child.Result);
                                if (result.Count == 0)
                                    goto loop;
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                    }
                }

                return new EvalTree
                {
                    Expression = expr,
                    Children = children,
                    Result = result
                };
            }

            public EvalTree VisitThisUsersetExpr(ThisUsersetExpression expr)
            {
                return new EvalTree
                {
                    Expression = expr,
                    Children = [],
                    Result = _service.GetRelations(_object)
                        .Select(x => x.User)
                        .ToHashSet()
                };
            }

            public EvalTree VisitTuplesetExpr(TuplesetExpression expr)
            {
                throw new NotImplementedException();
            }

            public EvalTree VisitTupleToUsersetExpr(TupleToUsersetExpression expr)
            {
                HashSet<AclRelation> tupleset = _service.GetRelations(new AclKey
                {
                    Namespace = expr.TuplesetExpression.Namespace ?? _object.Namespace,
                    Id = expr.TuplesetExpression.Object ?? _object.Id,
                    Relation = expr.TuplesetExpression.Relation ?? _object.Relation
                });

                var children = new List<EvalTree>();

                var result = new HashSet<AclKey>();

                foreach (var r in tupleset)
                {
                    string? @namespace = expr.ComputedUsersetExpression.Namespace ?? r.Object.Namespace;

                    if (@namespace == null)
                    {
                        throw new InvalidOperationException("Namespace cannot be null");
                    }

                    if (UsersetRef.TUPLE_USERSET_NAMESPACE.Equals(@namespace))
                    {
                        @namespace = r.User.Namespace;
                    }

                    string id = expr.ComputedUsersetExpression.Object ?? r.Object.Id;

                    if (UsersetRef.TUPLE_USERSET_OBJECT.Equals(id))
                    {
                        id = r.User.Id;
                    }

                    string relation = expr.ComputedUsersetExpression.Relation ?? r.Object.Relation;

                    if (UsersetRef.TUPLE_USERSET_RELATION.Equals(relation))
                    {
                        relation = r.User.Relation;
                    }

                    EvalTree eval = new Builder(_service, new AclKey
                    {
                        Namespace = @namespace,
                        Id = id,
                        Relation = relation
                    }).Eval();

                    children.Add(
                        new EvalTree
                        {
                            Expression = new ComputedUsersetExpression
                            {
                                Namespace = @namespace,
                                Object = id,
                                Relation = relation
                            },
                            Children = [eval],
                            Result = eval.Result
                        });

                    result.UnionWith(eval.Result);
                }

                return new EvalTree
                {
                    Expression = expr,
                    Children = children,
                    Result = result
                };
            }
        }
    }
}