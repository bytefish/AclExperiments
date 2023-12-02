using AclExperiments.Database;
using AclExperiments.Database.Connections;
using AclExperiments.Database.Model;
using AclExperiments.Stores;
using AclExperiments.Tests.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace AclExperiments.Tests.Stores
{
    [TestClass]
    public class SqlNamespaceConfigurationStoreTests : IntegrationTestBase
    {
        private INamespaceConfigurationStore _namespaceConfigurationStore = null!;

        protected override Task OnSetupBeforeCleanupAsync()
        {
            _namespaceConfigurationStore = _services.GetRequiredService<INamespaceConfigurationStore>();

            return Task.CompletedTask;
        }

        [TestMethod]
        public async Task GetLatestNamespaceConfiguration_MultipleNamespaceConfigurationVersions()
        {
            // Arrange
            await _namespaceConfigurationStore.AddNamespaceConfigurationAsync("doc", 1, "name: \"doc\"", 1, default);
            await _namespaceConfigurationStore.AddNamespaceConfigurationAsync("doc", 2, "name: \"test\"", 1, default);

            // Act
            var result = await _namespaceConfigurationStore.GetLatestNamespaceConfigurationAsync("doc", default);

            // Assert
            Assert.AreEqual("test", result.Name);
        }

        [TestMethod]
        public async Task GetLatestNamespaceConfiguration_ParsesNamespaceConfigurationCorrectly()
        {
            // Arrange
            await _namespaceConfigurationStore.AddNamespaceConfigurationAsync("doc", 1, File.ReadAllText("Resources/doc.nsconfig"), 1, default);

            // Act
            var result = await _namespaceConfigurationStore.GetLatestNamespaceConfigurationAsync("doc", default);

            // Assert
            Assert.AreEqual(result.Name, "doc");

            Assert.IsNotNull(result.Relations);

            Assert.IsTrue(result.Relations.ContainsKey("owner"));
            Assert.IsTrue(result.Relations.ContainsKey("editor"));
            Assert.IsTrue(result.Relations.ContainsKey("viewer"));
        }

        [TestMethod]
        public async Task RemoveNamespaceConfigurationAsync_DeletesNamespaceDocuments()
        {
            // Arrange
            await _namespaceConfigurationStore.AddNamespaceConfigurationAsync("doc", 1, "name: \"test\"", 1, default);

            // Act
            await _namespaceConfigurationStore.RemoveNamespaceConfigurationAsync("doc", 1, default);

            // Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _namespaceConfigurationStore.GetLatestNamespaceConfigurationAsync("doc", default));
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

            services.AddSingleton<INamespaceConfigurationStore, SqlNamespaceConfigurationStore>();
        }
    }
}
