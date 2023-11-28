using RebacExperiments.Acl.Eval;
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
                Name = "doc",
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

            var aclService = new AclService();

            // Parse and add Namespace Configurations.
            aclService.AddNamespaceConfigurations(
                NamespaceUsersetRewriteParser.Parse(text: File.ReadAllText("user.nsconfig")),
                NamespaceUsersetRewriteParser.Parse(text: File.ReadAllText("document.nsconfig")),
                NamespaceUsersetRewriteParser.Parse(text: File.ReadAllText("folder.nsconfig")));

            // Create the Test Case for checking the TupleToUserset Rule:

            // Folder 'folder_foo' is parent of Document 'doc_foo'.
            aclService.AddRelation(AclRelation.Parse("doc:doc_foo#parent@folder:folder_foo#..."));

            // User 'user1' is viewer of Folder 'folder_foo'.
            aclService.AddRelation(AclRelation.Parse("folder:folder_foo#viewer@user1"));

            // Find out who is viewer of doc_foo
            var result = EvalTreeFactory.Evaluate(aclService, new AclKey { Namespace = "doc", Id = null, Relation = "viewer" });
        }
    }
}