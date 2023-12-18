// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using AclExperiments.Expressions;
using AclExperiments.Parser.Generated;
using static AclExperiments.Parser.Generated.UsersetRewriteParser;

namespace AclExperiments.Parser
{
    public class NamespaceUsersetRewriteParser
    {
        public static NamespaceUsersetExpression Parse(string text)
        {
            var charStream = CharStreams.fromString(text);

            return Parse(charStream);
        }

        private static NamespaceUsersetExpression Parse(ICharStream input)
        {
            var parser = new UsersetRewriteParser(new CommonTokenStream(new UsersetRewriteLexer(input)));

            return (NamespaceUsersetExpression)new UsersetRewriteVisitor().Visit(parser.@namespace());
        }

        private class UsersetRewriteVisitor : UsersetRewriteBaseVisitor<UsersetExpression>
        {
            public override UsersetExpression VisitNamespace([NotNull] NamespaceContext context)
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

            public override UsersetExpression VisitRelation([NotNull] RelationContext context)
            {
                return new RelationUsersetExpression
                {
                    Name = Unquote(context.relationName.Text),
                    Rewrite = context.usersetRewrite() != null ? VisitUsersetRewrite(context.usersetRewrite()) : new ChildUsersetExpression { Userset = new ThisUsersetExpression() }
                };
            }

            public override UsersetExpression VisitUsersetRewrite([NotNull] UsersetRewriteContext context)
            {
                if (context.userset() == null)
                {
                    return new ChildUsersetExpression { Userset = new ThisUsersetExpression() };
                }

                return VisitUserset(context.userset());
            }

            public override UsersetExpression VisitChildUserset([NotNull] ChildUsersetContext context)
            {
                return new ChildUsersetExpression
                {
                    Userset = VisitUserset(context.userset())
                };
            }

            public override UsersetExpression VisitComputedUserset([NotNull] ComputedUsersetContext context)
            {
                string? @namespace = null;

                if (context.usersetNamespaceRef().Length > 1)
                {
                    throw new InvalidOperationException("More than one namespace specified");
                }

                if (context.usersetNamespaceRef().Length != 0)
                {
                    var usersetNamespaceRefContext = context.usersetNamespaceRef().First();

                    switch (usersetNamespaceRefContext.@ref.Type)
                    {
                        case STRING:
                            @namespace = Unquote(usersetNamespaceRefContext.STRING().GetText());
                            break;

                        case TUPLE_USERSET_NAMESPACE:
                            @namespace = UsersetRef.TUPLE_USERSET_NAMESPACE;
                            break;
                    }
                }

                string? @object = null;

                if (context.usersetObjectRef().Length > 1)
                {
                    throw new InvalidOperationException("More than one object specified");
                }

                if (context.usersetObjectRef().Length != 0)
                {
                    var usersetObjectRefContext = context.usersetObjectRef().First();

                    switch (usersetObjectRefContext.@ref.Type)
                    {
                        case STRING:
                            @object = Unquote(usersetObjectRefContext.STRING().GetText());
                            break;

                        case TUPLE_USERSET_OBJECT:
                            @object = UsersetRef.TUPLE_USERSET_OBJECT;
                            break;
                    }
                }

                if (@namespace == null && UsersetRef.TUPLE_USERSET_OBJECT.Equals(@object))
                {
                    @namespace = UsersetRef.TUPLE_USERSET_NAMESPACE;
                }

                string relation = string.Empty;

                if (context.usersetRelationRef().Length > 1)
                {
                    throw new InvalidOperationException("More than one relation specified");
                }

                if (context.usersetRelationRef().Length != 0)
                {
                    var usersetRelationRefContext = context.usersetRelationRef().First();

                    switch (usersetRelationRefContext.@ref.Type)
                    {
                        case STRING:
                            relation = Unquote(usersetRelationRefContext.STRING().GetText());
                            break;

                        case TUPLE_USERSET_RELATION:
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

            public override UsersetExpression VisitSetOperationUserset([NotNull] SetOperationUsersetContext context)
            {
                var op = context.op.Type switch
                {
                    UNION => SetOperationEnum.Union,
                    INTERSECT => SetOperationEnum.Intersect,
                    _ => throw new ArgumentException(nameof(context.op.Type)),
                };

                return new SetOperationUsersetExpression
                {
                    Operation = op,
                    Children = context.userset()
                        .Select(x => x.Accept(this))
                        .ToList()
                };
            }

            public override UsersetExpression VisitThisUserset([NotNull] ThisUsersetContext context)
            {
                return new ThisUsersetExpression();
            }

            public override UsersetExpression VisitTupleset([NotNull] TuplesetContext context)
            {
                string? @namespace = null;

                if (context.namespaceRef().Length > 1)
                {
                    throw new InvalidOperationException("More than one namespace specified");
                }

                if (context.namespaceRef().Length != 0)
                {
                    @namespace = Unquote(context.namespaceRef().First().@ref.Text);
                }

                string? @object = null;

                if (context.objectRef().Length > 1)
                {
                    throw new InvalidOperationException("More than one object specified");
                }

                if (context.objectRef().Length != 0)
                {
                    @object = Unquote(context.objectRef().First().@ref.Text);
                }

                string relation = string.Empty;

                if (context.relationRef().Length > 1)
                {
                    throw new InvalidOperationException("More than one relation specified");
                }

                if (context.relationRef().Length != 0)
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

            public override UsersetExpression VisitTupleToUserset([NotNull] TupleToUsersetContext context)
            {
                return new TupleToUsersetExpression
                {
                    TuplesetExpression = (TuplesetExpression)VisitTupleset(context.tupleset()),
                    ComputedUsersetExpression = (ComputedUsersetExpression)VisitComputedUserset(context.computedUserset())
                };
            }

            private static string Unquote(string value)
            {
                return value.Trim('"');
            }
        }
    }
}