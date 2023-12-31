# Experimenting with Relationship-based Access Control: Implementing the Google Zanzibar Check API and Expand API #

Google Zanzibar is Google's central solution for providing authorization among its many services 
and described in a paper at:

* [https://research.google/pubs/pub48190/](https://research.google/pubs/pub48190/)

This Git Repository implements a highly simplified version of the Google Zanzibar `Check API` 
and `Expand API`. It also includes an ANTLR4-based parser for the namespace configuration language 
described in the original paper.

The article for this repository can be found at:

* [https://www.bytefish.de/blog/acl_google_zanzibar.html](https://www.bytefish.de/blog/acl_google_zanzibar.html)

## Google Zanzibar "Check" and "Expand" Example ##

The Google Zanzibar paper describes a namespace configuration for documents. We expand on this 
example and add a namespace configuration for folders, and see how to use the Check API and Expand 
API implemented in the project.

### Namespace Configurations ###

In `Resources\doc.nsconfig` we define the namespace configuration for documents as ...

```
name: "doc"
    relation { name: "owner" }
    
    relation {
        name: "editor"

        userset_rewrite {
            union {
                child { _this {} }
                child { computed_userset { relation: "owner" } }
            } } }
    
    relation {
        name: "viewer"
        userset_rewrite {
            union {
                child { _this {} }
                child { computed_userset { relation: "editor" } }
                child { tuple_to_userset {
                    tupleset { 
                        relation: "parent" 
                    }
                    computed_userset {
                        object: $TUPLE_USERSET_OBJECT
                        relation: "viewer"
                } } }
} } }
```

In `Resources\folder.nsconfig` we define the namespace configuration for folders as ...

```
name: "folder"

relation { name: "parent" }

relation { name: "owner" }

relation {
  name: "editor"
  userset_rewrite {
    union {
      child { _this {} }
      child { computed_userset { relation: "owner" } }
}}}

relation {
  name: "viewer"
  userset_rewrite {
  union {
    child { _this {} }
    child { computed_userset { relation: "editor" } }
}}}
```

### Tests for the Check and Expand API ###

In the test we have one document `doc_1`, and two users `user_1` and `user_2`. `user_1` has a direct `viewer` 
relation to `doc_1` . `user_2` has a `viewer` permission on folder `folder_1`. The folder `folder_1` is `parent` 
of `doc_1`, and thus `user_2` inherits the `viewer` permission through the folders.

The database contains the following relation tuples.

```
Namespace |  Object       |   Relation    |   Subject             |
----------|---------------|---------------|-----------------------|
doc       |   doc_1       |   viewer      |   user_1              |
doc       |   doc_1       |   parent      |   folder:folder_1#... |
folder    |   folder_1    |   viewer      |   user_2              |
```

In the example you can see how to add and get namespace configurations, 

```csharp
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
            var subjectTree = await _aclService.ExpandAsync("doc", "doc_1", "viewer", 100, default);

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
```