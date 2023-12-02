using AclExperiments.Database;
using AclExperiments.Database.Model;
using AclExperiments.Stores;
using AclExperiments.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace AclExperiments.Tests.Stores
{
    public class SqlNamespaceConfigurationStoreTests : IntegrationTestBase
    {
        private INamespaceConfigurationStore _namespaceConfigurationStore = null!;

        protected override Task OnSetupBeforeCleanupAsync()
        {
            _namespaceConfigurationStore = _services.GetRequiredService<INamespaceConfigurationStore>();

            return Task.CompletedTask;
        }

        [Test]
        public async Task GetLatestNamespaceConfiguration_()
        {
            // Arrange
            await _namespaceConfigurationStore.AddNamespaceConfigurationAsync("doc", 1, "name: \"doc\"", 1, default);
            await _namespaceConfigurationStore.AddNamespaceConfigurationAsync("doc", 2, "name: \"test\"", 1, default);

            // Act
            var result = await _namespaceConfigurationStore.GetLatestNamespaceConfigurationAsync("doc", default);

            // Assert
            Assert.That(result.Name, Is.EqualTo("test"));
        }

        public override void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<INamespaceConfigurationStore, SqlNamespaceConfigurationStore>();
        }
    }
}
