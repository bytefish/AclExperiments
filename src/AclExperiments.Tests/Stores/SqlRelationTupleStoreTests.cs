using AclExperiments.Database.Connections;
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


        public override void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ISqlConnectionFactory>((sp) =>
            {
                var connectionString = _configuration.GetConnectionString("ApplicationDatabase");

                if (connectionString == null)
                {
                    throw new InvalidOperationException($"No Connection String named 'ApplicationDatabase' found in appsettings.json");
                }

                return new SqlServerConnectionFactory(connectionString);
            });

            services.AddSingleton<IRelationTupleStore, SqlRelationTupleStore>();
        }
    }
}
