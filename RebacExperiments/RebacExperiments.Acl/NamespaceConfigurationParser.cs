using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using RebacExperiments.Acl.Ast.Generated;
using RebacExperiments.Acl.Model;

namespace RebacExperiments.Acl
{
    public class NamespaceConfigurationParser
    {
        public static NamespaceUsersetExpression Parse(string text)
        {
            var charStream = CharStreams.fromString(text);

            return Parse(charStream);
        }

        private static NamespaceUsersetExpression Parse(ICharStream input)
        {
            UsersetRewriteParser parser = new UsersetRewriteParser(new CommonTokenStream(new UsersetRewriteLexer(input)));

            return (NamespaceUsersetExpression)new Builder().Visit(parser.@namespace());
        }

        private class Builder : UsersetRewriteBaseVisitor<UsersetExpression>
        {
            public override UsersetExpression VisitNamespace([NotNull] UsersetRewriteParser.NamespaceContext context)
            {
                return new NamespaceUsersetExpression
                {

                    Name = Unquote(context.namespaceName.Text),
                    Relations = context.relation()
                        .Select(VisitRelation)
                        .Cast<RelationUsersetExpression>()
                        .ToDictionary(x => x.Name, x => x)
                };
            }

            public override UsersetExpression VisitRelation([NotNull] UsersetRewriteParser.RelationContext context)
            {
                return new RelationUsersetExpression
                {
                    Name = Unquote(context.relationName.Text),
                    Rewrite = context.usersetRewrite() != null ? VisitUsersetRewrite(context.usersetRewrite()) : null
                };
            }

            public override UsersetExpression VisitUsersetRewrite([NotNull] UsersetRewriteParser.UsersetRewriteContext context)
            {
                if(context.userset() == null)
                {
                    return new ChildUsersetExpression { Userset = new ThisUsersetExpression() };
                }

                return VisitUserset(context.userset());
            }

            public override UsersetExpression VisitChildUserset([NotNull] UsersetRewriteParser.ChildUsersetContext context)
            {
                return new ChildUsersetExpression
                {
                    Userset = VisitUserset(context.userset())
                };
            }

            public override UsersetExpression VisitComputedUserset([NotNull] UsersetRewriteParser.ComputedUsersetContext context)
            {
                string? @namespace = null;

                if (context.usersetNamespaceRef().Length > 1)
                {
                    throw new InvalidOperationException("More than one namespace specified"); //TODO: figure out which exception to throw
                }

                if (context.usersetNamespaceRef().Any())
                {
                    var usersetNamespaceRefContext = context.usersetNamespaceRef().First();

                    switch (usersetNamespaceRefContext.@ref.Type)
                    {
                        case UsersetRewriteParser.STRING:
                            @namespace = Unquote(usersetNamespaceRefContext.STRING().GetText());
                            break;

                        case UsersetRewriteParser.TUPLE_USERSET_NAMESPACE:
                            @namespace = UsersetRef.TUPLE_USERSET_NAMESPACE;
                            break;
                    }
                }

                string? @object = null;

                if (context.usersetObjectRef().Length > 1)
                {
                    throw new InvalidOperationException("More than one object specified"); //TODO: figure out which exception to throw
                }

                if (context.usersetObjectRef().Any())
                {
                    var usersetObjectRefContext = context.usersetObjectRef().First();

                    switch (usersetObjectRefContext.@ref.Type)
                    {
                        case UsersetRewriteParser.STRING:
                            @object = Unquote(usersetObjectRefContext.STRING().GetText());
                            break;

                        case UsersetRewriteParser.TUPLE_USERSET_OBJECT:
                            @object = UsersetRef.TUPLE_USERSET_OBJECT;
                            break;
                    }
                }

                string relation = string.Empty;

                if (context.usersetRelationRef().Length > 1)
                {
                    throw new InvalidOperationException("More than one relation specified"); //TODO: figure out which exception to throw
                }
                if (context.usersetRelationRef().Any())
                {
                    var usersetRelationRefContext = context.usersetRelationRef().First();
                    switch (usersetRelationRefContext.@ref.Type)
                    {
                        case UsersetRewriteParser.STRING:
                            relation = Unquote(usersetRelationRefContext.STRING().GetText());
                            break;

                        case UsersetRewriteParser.TUPLE_USERSET_RELATION:
                            relation = UsersetRef.TUPLE_USERSET_RELATION;
                            break;
                    }
                }

                return new ComputedUsersetExpression
                {
                    Namespace = @namespace,
                    Object = @object,
                    Relation = relation
                };
            }

            public override UsersetExpression VisitNamespaceRef([NotNull] UsersetRewriteParser.NamespaceRefContext context)
            {
                return base.VisitNamespaceRef(context);
            }

            public override UsersetExpression VisitObjectRef([NotNull] UsersetRewriteParser.ObjectRefContext context)
            {
                return base.VisitObjectRef(context);
            }


            public override UsersetExpression VisitRelationRef([NotNull] UsersetRewriteParser.RelationRefContext context)
            {
                return base.VisitRelationRef(context);
            }

            public override UsersetExpression VisitSetOperationUserset([NotNull] UsersetRewriteParser.SetOperationUsersetContext context)
            {
                SetOperationEnum op;

                switch (context.op.Type)
                {
                    case UsersetRewriteParser.UNION:
                        op = SetOperationEnum.Union;
                        break;
                    case UsersetRewriteParser.INTERSECT:
                        op = SetOperationEnum.Intersect;
                        break;
                    case UsersetRewriteParser.EXCLUDE:
                        op = SetOperationEnum.Exclude;
                        break;
                    default:
                        throw new ArgumentException(nameof(op));
                }

                return new SetOperationUsersetExpression
                {
                    Operation = op,
                    Children = context.userset()
                        .Select(x => x.Accept(this))
                        .ToList()
                };
            }

            public override UsersetExpression VisitThisUserset([NotNull] UsersetRewriteParser.ThisUsersetContext context)
            {
                return new ThisUsersetExpression();
            }

            public override UsersetExpression VisitTupleset([NotNull] UsersetRewriteParser.TuplesetContext context)
            {
                string? @namespace = null;

                if (context.namespaceRef().Length > 1) 
                {
                    throw new InvalidOperationException("More than one namespace specified"); //TODO: figure out which exception to throw
                }

                if (context.namespaceRef().Any())
                {
                    @namespace = Unquote(context.namespaceRef().First().@ref.Text);
                }
                
                string? @object = null;

                if (context.objectRef().Length > 1)
                {
                    throw new InvalidOperationException("More than one object specified"); //TODO: figure out which exception to throw
                }

                if (context.objectRef().Any())
                {
                    @object = Unquote(context.objectRef().First().@ref.Text);
                }
                
                string relation = string.Empty;
                
                if (context.relationRef().Length > 1)
                {
                    throw new InvalidOperationException("More than one relation specified"); //TODO: figure out which exception to throw
                }
                
                if (context.relationRef().Any())
                {
                    relation = Unquote(context.relationRef().First().@ref.Text);
                }

                return new TuplesetExpression
                {
                    Namespace = @namespace,
                    Object = @object,
                    Relation = relation
                };
            }

            public override UsersetExpression VisitTupleToUserset([NotNull] UsersetRewriteParser.TupleToUsersetContext context)
            {
                return new TupleToUsersetExpression
                {
                    TuplesetExpression = (TuplesetExpression)VisitTupleset(context.tupleset()),
                    ComputedUsersetExpression = (ComputedUsersetExpression)VisitComputedUserset(context.computedUserset())
                };
            }

            public override UsersetExpression VisitUserset([NotNull] UsersetRewriteParser.UsersetContext context)
            {
                return base.VisitUserset(context);
            }

            public override UsersetExpression VisitUsersetNamespaceRef([NotNull] UsersetRewriteParser.UsersetNamespaceRefContext context)
            {
                return base.VisitUsersetNamespaceRef(context);
            }

            public override UsersetExpression VisitUsersetObjectRef([NotNull] UsersetRewriteParser.UsersetObjectRefContext context)
            {
                return base.VisitUsersetObjectRef(context);
            }

            public override UsersetExpression VisitUsersetRelationRef([NotNull] UsersetRewriteParser.UsersetRelationRefContext context)
            {
                return base.VisitUsersetRelationRef(context);
            }

            private static string Unquote(string value)
            {
                return value.Trim('"');
            }
        }
    }
}