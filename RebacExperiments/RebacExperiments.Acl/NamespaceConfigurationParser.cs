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
                return base.VisitUsersetRewrite(context);
            }

            public override UsersetExpression VisitChildUserset([NotNull] UsersetRewriteParser.ChildUsersetContext context)
            {
                return base.VisitChildUserset(context);
            }

            public override UsersetExpression VisitComputedUserset([NotNull] UsersetRewriteParser.ComputedUsersetContext context)
            {
                return base.VisitComputedUserset(context);
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
                return base.VisitSetOperationUserset(context);
            }

            public override UsersetExpression VisitThisUserset([NotNull] UsersetRewriteParser.ThisUsersetContext context)
            {
                return base.VisitThisUserset(context);
            }

            public override UsersetExpression VisitTupleset([NotNull] UsersetRewriteParser.TuplesetContext context)
            {
                return base.VisitTupleset(context);
            }

            public override UsersetExpression VisitTupleToUserset([NotNull] UsersetRewriteParser.TupleToUsersetContext context)
            {
                return base.VisitTupleToUserset(context);
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