// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AclExperiments.Tests.Models
{
    [TestClass]
    public class AuthorizationModelSerializeTests
    {
        [TestMethod]
        public void AuthorizationModel_Deserialize()
        {
            // User Namespace
            var ns_user = new NamespaceUsersetExpression
            {
                Name = "user",
                Version = 1,
            };

            // Document Namespace
            var ns_doc = new NamespaceUsersetExpression
            {
                Name = "doc",
                Version = 1,
                Metadata = new MetadataExpression
                {
                    Relations =
                    {
                        { 
                            "owner", 
                            new MetadataRelationExpression 
                            {
                                DirectlyRelatedTypes = 
                                [ 
                                    new DirectlyRelatedType { Namespace = "user" } 
                                ]
                            }
                        },
                        { 
                            "parent", 
                            new MetadataRelationExpression 
                            {
                                DirectlyRelatedTypes = 
                                [ 
                                    new DirectlyRelatedType { Namespace = "folder" } 
                                ]
                            }
                        },
                        { 
                            "viewer", 
                            new MetadataRelationExpression 
                            {
                                DirectlyRelatedTypes = 
                                [ 
                                    new DirectlyRelatedType { Namespace = "user" } 
                                ]
                            }
                        },
                    }
                },
                Relations = 
                {
                    { 
                        "parent", 
                        new ThisUsersetExpression() 
                    },
                    { 
                        "owner", 
                        new ThisUsersetExpression() 
                    },
                    { 
                        "editor", 
                        new SetOperationUsersetExpression
                        {
                            Operation = SetOperationEnum.Union,
                            Children =
                            [
                                new ThisUsersetExpression(),
                                new ComputedUsersetExpression
                                {
                                    Relation = "owner"
                                }
                            ]
                        }
                    },
                    { 
                        "viewer", 
                        new SetOperationUsersetExpression
                        {
                            Operation = SetOperationEnum.Union,
                            Children = [
                                new ThisUsersetExpression(),
                                new ComputedUsersetExpression
                                {
                                    Relation = "owner"
                                },
                                new TupleToUsersetExpression
                                {
                                    TuplesetExpression = new TuplesetExpression
                                    {
                                        Relation = "parent"
                                    },
                                    ComputedUsersetExpression = new ComputedUsersetExpression
                                    {
                                        Relation = "viewer"
                                    }
                                }
                            ]
                        }
                    }
                }
            };

            // Folder Namespace
            var ns_folder = new NamespaceUsersetExpression
            {
                Name = "folder",
                Version = 1,
                Metadata = new MetadataExpression
                {
                    Relations =
                    {
                        {
                            "owner",
                            new MetadataRelationExpression
                            {
                                DirectlyRelatedTypes =
                                [
                                    new DirectlyRelatedType
                                    {
                                        Namespace = "user"
                                    }
                                ]
                            }
                        },
                        {
                            "parent",
                            new MetadataRelationExpression
                            {
                                DirectlyRelatedTypes = 
                                [ 
                                    new DirectlyRelatedType 
                                    { 
                                        Namespace = "folder" 
                                    } 
                                ]
                            }
                        },
                        {
                            "viewer",
                            new MetadataRelationExpression
                            {
                                DirectlyRelatedTypes =
                                [
                                    new DirectlyRelatedType
                                    {
                                        Namespace = "user"
                                    }
                                ]
                            }
                        }
                    }
                },
                Relations =
                {
                    {
                        "parent",
                        new ThisUsersetExpression()
                    },
                    { 
                        "owner", 
                        new ThisUsersetExpression() 
                    },
                    { 
                        "editor", 
                        new SetOperationUsersetExpression
                        {
                            Operation = SetOperationEnum.Union,
                            Children =
                            [
                                new ThisUsersetExpression(),
                                new ComputedUsersetExpression
                                {
                                    Relation = "owner"
                                }
                            ]
                        }
                    },
                    { 
                        "viewer", 
                        new SetOperationUsersetExpression
                        {
                            Operation = SetOperationEnum.Union,
                            Children = 
                            [
                                new ThisUsersetExpression(),
                                new ComputedUsersetExpression
                                {
                                    Relation = "owner"
                                },
                                new TupleToUsersetExpression
                                {
                                    TuplesetExpression = new TuplesetExpression
                                    {
                                        Relation = "parent"
                                    },
                                    ComputedUsersetExpression = new ComputedUsersetExpression
                                    {
                                        Relation = "viewer"
                                    }
                                }
                            ]
                        }
                    }
                }
            };

            // Deserialize the Authorization Model
            var auth_model = JsonSerializer.Deserialize<AuthorizationModel>(json: File.ReadAllText("Resources/google-drive.json"))!;

            // Check User

            {
                var auth_model_user = auth_model.Namespaces.First(x => x.Name == "user");

                Assert.AreEqual(ns_user.Relations.Count, auth_model_user.Relations.Count);
            }
            {
                var auth_model_doc = auth_model.Namespaces.First(x => x.Name == "doc");

                Assert.AreEqual(ns_doc.Relations.Count, auth_model_doc.Relations.Count);
            }
            {
                var auth_model_folder = auth_model.Namespaces.First(x => x.Name == "folder");

                Assert.AreEqual(ns_folder.Relations.Count, auth_model_folder.Relations.Count);
            }
        }
    }
}