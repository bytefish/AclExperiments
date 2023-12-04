// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Expressions;
using AclExperiments.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AclExperiments.Tests.Parser
{
    [TestClass]
    public class NamespaceUsersetRewriteParserTests
    {
        /// <summary>
        /// Parses the namespace configuration described in Google Zanzibar paper:
        /// 
        ///         ﻿name: "doc"
        ///             
        ///             relation { name: "owner" }
        ///         
        ///             relation {
        ///                 name: "editor"
        ///         
        ///                 userset_rewrite {
        ///                     union {
        ///                         child { _this {} }
        ///                         child { computed_userset { relation: "owner" } }
        ///                     } } }
        ///             
        ///             relation {
        ///                 name: "viewer"
        ///                 userset_rewrite {
        ///                     union {
        ///                         child { _this {} }
        ///                         child { computed_userset { relation: "editor" } }
        ///                         child { tuple_to_userset {
        ///                             tupleset { 
        ///                                 relation: "parent"
        ///                             }
        ///                             computed_userset {
        ///                                 object: $TUPLE_USERSET_OBJECT
        ///                                 relation: "viewer"
        ///                         } } }
        ///         } } }
        /// </summary>
        [TestMethod]
        public void NamespaceUsersetRewriteParser_GoogleZanzibarExample_CheckAstBasic()
        {
            // Arrange
            var namespaceConfigText = File.ReadAllText("./Resources/doc.nsconfig");

            // Act
            var namespaceConfig = NamespaceUsersetRewriteParser.Parse(namespaceConfigText);

            // Assert
            Assert.AreEqual("doc", namespaceConfig.Name);

            Assert.AreEqual(3, namespaceConfig.Relations.Count);

            Assert.AreEqual(true, namespaceConfig.Relations.ContainsKey("owner"));
            Assert.AreEqual(true, namespaceConfig.Relations.ContainsKey("editor"));
            Assert.AreEqual(true, namespaceConfig.Relations.ContainsKey("viewer"));
        }

        /// <summary>
        /// Parses the namespace configuration described in Google Zanzibar paper:
        /// 
        ///         ﻿name: "doc"
        ///             
        ///             relation { name: "owner" }
        ///         
        ///             relation {
        ///                 name: "editor"
        ///         
        ///                 userset_rewrite {
        ///                     union {
        ///                         child { _this {} }
        ///                         child { computed_userset { relation: "owner" } }
        ///                     } } }
        ///             
        ///             relation {
        ///                 name: "viewer"
        ///                 userset_rewrite {
        ///                     union {
        ///                         child { _this {} }
        ///                         child { computed_userset { relation: "editor" } }
        ///                         child { tuple_to_userset {
        ///                             tupleset { 
        ///                                 relation: "parent"
        ///                             }
        ///                             computed_userset {
        ///                                 object: $TUPLE_USERSET_OBJECT
        ///                                 relation: "viewer"
        ///                         } } }
        ///         } } }
        ///
        /// We expect 3 Relations: "owner", "editor" and "viewer".
        /// </summary>
        [TestMethod]
        public void NamespaceUsersetRewriteParser_GoogleZanzibarExample_CheckAst()
        {
            // Arrange
            var namespaceConfigText = File.ReadAllText("./Resources/doc.nsconfig");

            // Act
            var namespaceConfig = NamespaceUsersetRewriteParser.Parse(namespaceConfigText);

            // Assert
            Assert.AreEqual("doc", namespaceConfig.Name);

            Assert.AreEqual(3, namespaceConfig.Relations.Count);

            Assert.AreEqual(true, namespaceConfig.Relations.ContainsKey("owner"));
            Assert.AreEqual(true, namespaceConfig.Relations.ContainsKey("editor"));
            Assert.AreEqual(true, namespaceConfig.Relations.ContainsKey("viewer"));

            Assert.AreEqual("owner", namespaceConfig.Relations["owner"].Name);
            {
                var childUsersetExpression = namespaceConfig.Relations["owner"].Rewrite as ChildUsersetExpression;
                {
                    var thisUsersetExpression = childUsersetExpression!.Userset as ThisUsersetExpression;

                    Assert.IsNotNull(thisUsersetExpression);
                }
            }

            Assert.AreEqual("editor", namespaceConfig.Relations["editor"].Name);
            {
                var setOperationUsersetExpression = namespaceConfig.Relations["editor"].Rewrite as SetOperationUsersetExpression;

                Assert.AreEqual(SetOperationEnum.Union, setOperationUsersetExpression!.Operation);
                Assert.AreEqual(2, setOperationUsersetExpression!.Children.Count);
                {
                    var firstLeafNode = ((ChildUsersetExpression)setOperationUsersetExpression!.Children[0]).Userset as ThisUsersetExpression;
                    var secondLeafNode = ((ChildUsersetExpression)setOperationUsersetExpression!.Children[1]).Userset as ComputedUsersetExpression;

                    Assert.IsNotNull(firstLeafNode);

                    Assert.IsNotNull(secondLeafNode);
                    Assert.AreEqual(null, secondLeafNode.Namespace);
                    Assert.AreEqual(null, secondLeafNode.Object);
                    Assert.AreEqual("owner", secondLeafNode.Relation);
                }
            }

            Assert.AreEqual("viewer", namespaceConfig.Relations["viewer"].Name);
            {
                var setOperationUsersetExpression = namespaceConfig.Relations["viewer"].Rewrite as SetOperationUsersetExpression;

                Assert.AreEqual(SetOperationEnum.Union, setOperationUsersetExpression!.Operation);

                Assert.AreEqual(3, setOperationUsersetExpression!.Children.Count);
                {
                    var leafNode1 = ((ChildUsersetExpression)setOperationUsersetExpression!.Children[0]).Userset as ThisUsersetExpression;
                    var leafNode2 = ((ChildUsersetExpression)setOperationUsersetExpression!.Children[1]).Userset as ComputedUsersetExpression;
                    var leafNode3 = ((ChildUsersetExpression)setOperationUsersetExpression!.Children[2]).Userset as TupleToUsersetExpression;

                    Assert.IsNotNull(leafNode1);
                    Assert.IsNotNull(leafNode2);
                    Assert.IsNotNull(leafNode3);

                    Assert.AreEqual(null, leafNode2.Namespace);
                    Assert.AreEqual(null, leafNode2.Object);
                    Assert.AreEqual("editor", leafNode2.Relation);


                    Assert.AreEqual("parent", leafNode3.TuplesetExpression.Relation);
                    
                    Assert.AreEqual(UsersetRef.TUPLE_USERSET_NAMESPACE, leafNode3.ComputedUsersetExpression.Namespace);
                    Assert.AreEqual(UsersetRef.TUPLE_USERSET_OBJECT, leafNode3.ComputedUsersetExpression.Object);
                    Assert.AreEqual("viewer", leafNode3.ComputedUsersetExpression.Relation);
                }
            }
        }
    }
}