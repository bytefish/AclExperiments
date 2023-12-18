// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Database.Model;
using AclExperiments.Models;
using AclExperiments.Stores;
using AclExperiments.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AclExperiments.Tests
{
    [TestClass]
    public class AclServiceTests : IntegrationTestBase
    {
        private AclService _aclService = null!;

        private IAuthorizationModelStore _authorizationModelStore = null!;
        private IRelationTupleStore _relationTupleStore = null!;

        protected override Task OnSetupBeforeCleanupAsync()
        {
            _aclService = _services.GetRequiredService<AclService>();
            _relationTupleStore = _services.GetRequiredService<IRelationTupleStore>();
            _authorizationModelStore = _services.GetRequiredService<IAuthorizationModelStore>();

            return Task.CompletedTask;
        }

        public override void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<AclService>();
            services.AddSingleton<IAuthorizationModelStore, SqlAuthorizationModelStore>();
            services.AddSingleton<IRelationTupleStore, SqlRelationTupleStore>();
        }

        #region Check API

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
        public async Task CheckAsync_CheckUserPermissions()
        {
            // Arrange
            var authorizationModel = new SqlAuthorizationModel
            {
                ModelKey = "google-drive",
                Name = "Google Drive",
                Description = "An Authorization Model for Google Drive",
                Content = File.ReadAllText("Resources/google-drive.json"),
                LastEditedBy = 1
            };

            using (var context = _dbContextFactory.CreateDbContext())
            {
                await context.AddAsync(authorizationModel, default);
                await context.SaveChangesAsync(default);
            }

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
                            Id = "doc_1"
                        },
                        Relation = "parent",
                        Subject = new AclSubjectSet
                        {

                            Namespace = "folder",
                            Object = "folder_1",
                            Relation = "..."
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

            // Act
            var user_1_is_permitted = await _aclService.CheckAsync("google-drive", "doc", "doc_1", "viewer", "user:user_1", default);
            var user_2_is_permitted = await _aclService.CheckAsync("google-drive", "doc", "doc_1", "viewer", "user:user_2", default);
            var user_3_is_permitted = await _aclService.CheckAsync("google-drive", "doc", "doc_1", "viewer", "user:user_3", default);

            // Assert
            Assert.AreEqual(true, user_1_is_permitted);
            Assert.AreEqual(true, user_2_is_permitted);
            Assert.AreEqual(false, user_3_is_permitted);
        }

        #endregion Check API

        #region Expand API

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
        public async Task Expand_ExpandUsersetRewrites()
        {
            // Arrange
            var authorizationModel = new SqlAuthorizationModel
            {
                ModelKey = "google-drive",
                Name = "Google Drive",
                Description = "An Authorization Model for Google Drive",
                Content = File.ReadAllText("Resources/google-drive.json"),
                LastEditedBy = 1
            };

            using (var context = _dbContextFactory.CreateDbContext())
            {
                await context.AddAsync(authorizationModel, default);
                await context.SaveChangesAsync(default);
            }

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
                            Id = "doc_1"
                        },
                        Relation = "parent",
                        Subject = new AclSubjectSet
                        {

                            Namespace = "folder",
                            Object = "folder_1",
                            Relation = "..."
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

            // Act
            var subjectTree = await _aclService.ExpandAsync("google-drive", "doc", "doc_1", "viewer", 100, default);

            // Assert
            Assert.AreEqual(2, subjectTree.Result.Count);

            var sortedSubjectTreeResults = subjectTree.Result
                .Cast<AclSubjectId>()
                .OrderBy(x => x.Id)
                .ToList();

            Assert.AreEqual("user_1", sortedSubjectTreeResults[0].Id);
            Assert.AreEqual("user_2", sortedSubjectTreeResults[1].Id);
        }

        #endregion Expand API

    }
}