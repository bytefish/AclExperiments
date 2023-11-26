using RebacExperiments.Acl.Model;
using RebacExperiments.Acl.Parser;

namespace RebacExperiments.Acl.Tests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var configuration = new NamespaceUsersetExpression
            {
                Name = "document",
                Relations =
                {
                    {
                        "owner",
                        new RelationUsersetExpression { Name = "owner" }
                    },
                    {
                        "editor",
                        new RelationUsersetExpression
                        {
                            Name = "editor",
                            Rewrite = new SetOperationUsersetExpression
                            {
                                Operation = SetOperationEnum.Union,
                                Children =
                                [
                                    new ThisUsersetExpression(),
                                    new ComputedUsersetExpression { Relation = "owner" }
                                ]
                            }
                        }
                    },
                    {
                        "viewer",
                        new RelationUsersetExpression
                        {
                            Name = "viewer",
                            Rewrite = new SetOperationUsersetExpression
                            {
                                Operation = SetOperationEnum.Union,
                                Children =
                                [
                                    new ThisUsersetExpression(),
                                    new ComputedUsersetExpression { Relation = "editor" },
                                    new TupleToUsersetExpression
                                    {
                                        TuplesetExpression = new TuplesetExpression { Relation = "parent" },
                                        ComputedUsersetExpression = new ComputedUsersetExpression
                                        {
                                            Object = " $TUPLE_USERSET_OBJECT",
                                            Relation = "viewer"
                                        }
                                    }
                                ]
                            }
                        }
                    }
                }
            };


            var a = File.ReadAllText("document.nsconfig");

            var result = NamespaceUsersetRewriteParser.Parse(a);
        }
    }
}