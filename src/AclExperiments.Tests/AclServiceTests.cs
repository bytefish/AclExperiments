// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments;
using AclExperiments.Database.Connections;
using AclExperiments.Models;
using AclExperiments.Stores;
using AclExperiments.Tests.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AclExperiments.Tests
{
    [TestClass]
    public class AclServiceTests : IntegrationTestBase
    {
        private AclService _aclService = null!;

        private INamespaceConfigurationStore _namespaceConfigurationStore = null!;
        private IRelationTupleStore _relationTupleStore = null!;
        
        protected override Task OnSetupBeforeCleanupAsync()
        {
            _aclService = _services.GetRequiredService<AclService>();
            _relationTupleStore = _services.GetRequiredService<IRelationTupleStore>();
            _namespaceConfigurationStore = _services.GetRequiredService<INamespaceConfigurationStore>();

            return Task.CompletedTask;
        }


        ///// <summary>
        /// In this test we have one document "doc_1", and two users "user_1" and "user_2". "user_1" 
        /// has a "viewer" permission on "doc_1", because he has a direct relationto it. "user_2" has 
        /// a viewer permission on a folder "folder_1". 
        /// 
        /// The folder "folder_1" is a "parent" of the document, and thus "user_2" inherits the folders 
        /// permission through the computed userset.
        /// 
        /// Namespace |  Object       |   Relation    |   Subject             |
        /// ----------|---------------|---------------|-----------------------|
        /// doc       |   doc_1       |   viewer      |   user_1              |
        /// doc       |   doc_1       |   parent      |   folder:folder_1#... |
        /// folder    |   folder_1    |   viewer      |   user_2              |
        /// </summary>
        [TestMethod]
        public async Task GetRelationTuplesAsync_QueryForNamespace()
        {
            // Arrange
            await _namespaceConfigurationStore.AddNamespaceConfigurationAsync("doc", 1, File.ReadAllText("Resources/doc.nsconfig"), 1, default);
            await _namespaceConfigurationStore.AddNamespaceConfigurationAsync("folder", 1, File.ReadAllText("Resources/folder.nsconfig"), 1, default);

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
                            Id = "doc_1"
                        },
                        Relation = "parent",
                        Subject = new AclSubjectId
                        {
                            Id = "folder:folder_1#..."
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

            // Act
            var user_1_is_permitted = await _aclService.CheckAsync("doc", "doc_1", "viewer", "user_1", default);
            var user_2_is_permitted = await _aclService.CheckAsync("doc", "doc_1", "viewer", "user_2", default);
            var user_3_is_permitted = await _aclService.CheckAsync("doc", "doc_1", "viewer", "user_3", default);

            // Assert
            Assert.AreEqual(true, user_1_is_permitted);
            Assert.AreEqual(true, user_2_is_permitted);
            Assert.AreEqual(false, user_3_is_permitted);
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

            services.AddSingleton<AclService>();
            services.AddSingleton<INamespaceConfigurationStore, SqlNamespaceConfigurationStore>();
            services.AddSingleton<IRelationTupleStore, SqlRelationTupleStore>();
        }
    }
}