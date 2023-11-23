using RebacExperiments.Server.Api.Infrastructure.Acl.Rewrite;

namespace RebacExperiments.Server.Api.Tests
{
    public class GoogleZanzibarTests
    {
        public void NamespaceConfigurationExample()
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
        }
    }
}