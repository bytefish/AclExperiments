using AclExperiments.Models;
using AclExperiments.Stores;
using AclExperiments.Tests.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AclExperiments.Tests.Stores
{
    [TestClass]
    public class SqlRelationTupleStoreTests : IntegrationTestBase
    {
        private IRelationTupleStore _relationTupleStore = null!;

        protected override Task OnSetupBeforeCleanupAsync()
        {
            _relationTupleStore = _services.GetRequiredService<IRelationTupleStore>();

            return Task.CompletedTask;
        }

        [TestMethod]
        public async Task GetRelationTuplesAsync_QueryForNamespace()
        {
            // Arrange
            var aclRelations = new[]
            {
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "doc",
                        Id = "doc_1"
                    },
                    Relation = "owner",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_1"
                    }
                },
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "doc",
                        Id = "doc_2"
                    },
                    Relation = "owner",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_2"
                    }
                },
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "folder",
                        Id = "folder_1"
                    },
                    Relation = "viewer",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_2"
                    }
                },
            };

            await _relationTupleStore.AddRelationTuplesAsync(aclRelations, 1, default);

            var query = new RelationTupleQuery
            {
                Namespace = "doc"
            };

            // Act
            var results = await _relationTupleStore.GetRelationTuplesAsync(query, default);

            // Assert
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public async Task GetRelationTuplesAsync_QueryForId()
        {
            // Arrange
            var aclRelations = new[]
            {
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "doc",
                        Id = "doc_1"
                    },
                    Relation = "owner",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_1"
                    }
                },
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "doc",
                        Id = "doc_2"
                    },
                    Relation = "owner",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_2"
                    }
                },
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "folder",
                        Id = "folder_1"
                    },
                    Relation = "viewer",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_2"
                    }
                },
            };

            await _relationTupleStore.AddRelationTuplesAsync(aclRelations, 1, default);

            var query = new RelationTupleQuery
            {
                Object = "doc_1"
            };

            // Act
            var results = await _relationTupleStore.GetRelationTuplesAsync(query, default);

            // Assert
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public async Task GetRelationTuplesAsync_QueryForRelation()
        {
            // Arrange
            var aclRelations = new[]
            {
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "doc",
                        Id = "doc_1"
                    },
                    Relation = "owner",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_1"
                    }
                },
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "doc",
                        Id = "doc_2"
                    },
                    Relation = "owner",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_2"
                    }
                },
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "folder",
                        Id = "folder_1"
                    },
                    Relation = "viewer",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_2"
                    }
                },
            };

            await _relationTupleStore.AddRelationTuplesAsync(aclRelations, 1, default);

            var query = new RelationTupleQuery
            {
                Relation = "owner"
            };

            // Act
            var results = await _relationTupleStore.GetRelationTuplesAsync(query, default);

            // Assert
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public async Task GetRelationTuplesAsync_OneSubject()
        {
            // Arrange
            var aclRelations = new[]
            {
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "doc",
                        Id = "doc_1"
                    },
                    Relation = "owner",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_1"
                    }
                },
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "doc",
                        Id = "doc_2"
                    },
                    Relation = "owner",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_2"
                    }
                },
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "folder",
                        Id = "folder_1"
                    },
                    Relation = "viewer",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_2"
                    }
                },
            };

            await _relationTupleStore.AddRelationTuplesAsync(aclRelations, 1, default);

            var user2 = new AclSubjectId
            {
                Namespace = "user",
                Id = "user_2"
            };

            // Act
            var results = await _relationTupleStore.GetRelationsByObjectNamespaceAsync("folder", "viewer", [user2], default);

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("folder", results[0].Object.Namespace);
            Assert.AreEqual("folder_1", results[0].Object.Id);
        }

        [TestMethod]
        public async Task GetRelationTuplesAsync_MultipleSubjects()
        {
            // Arrange
            var aclRelations = new[]
            {
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "doc",
                        Id = "doc_1"
                    },
                    Relation = "owner",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_1"
                    }
                },
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "doc",
                        Id = "doc_2"
                    },
                    Relation = "owner",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_2"
                    }
                },
                new AclRelation
                {
                    Object = new AclObject
                    {
                        Namespace = "folder",
                        Id = "folder_1"
                    },
                    Relation = "viewer",
                    Subject = new AclSubjectId
                    {
                        Namespace = "user",
                        Id = "user_2"
                    }
                },
            };

            await _relationTupleStore.AddRelationTuplesAsync(aclRelations, 1, default);

            var user1 = new AclSubjectId
            {
                Namespace = "user",
                Id = "user_1"
            };

            var user2 = new AclSubjectId
            {
                Namespace = "user",
                Id = "user_2"
            };

            // Act
            var results = await _relationTupleStore.GetRelationsByObjectNamespaceAsync("doc", "owner", [user1, user2], default);

            // Assert
            Assert.AreEqual(2, results.Count);

            var objects = results
                .Select(x => x.Subject)
                .Cast<AclSubjectId>()
                .OrderBy(x => x.Id)
                .ToList();

            Assert.AreEqual("user_1", objects[0].Id);
            Assert.AreEqual("user_2", objects[1].Id);
        }

        public override void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IRelationTupleStore, SqlRelationTupleStore>();
        }
    }
}
