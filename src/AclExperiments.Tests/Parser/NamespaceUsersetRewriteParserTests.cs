// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AclExperiments.Tests.Parser
{
    [TestClass]
    public class NamespaceUsersetRewriteParserTests
    {
        [TestMethod]
        public void a()
        {
            {

            }
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

            var ns_user = new NamespaceUsersetExpression
            {
                Name = "user",
                Version = 1,
            };

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

            var settings = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            var ns_user_json = JsonSerializer.Serialize(ns_user, settings);
            var ns_folder_json = JsonSerializer.Serialize(ns_folder, settings);
            var ns_doc_json = JsonSerializer.Serialize(ns_doc, settings);
        }
    }
}