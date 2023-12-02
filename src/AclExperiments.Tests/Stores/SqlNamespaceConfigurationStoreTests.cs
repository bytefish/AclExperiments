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
        private INamespaceConfigurationStore _namespaceConfigurationStore;

        protected override Task OnSetupBeforeCleanupAsync()
        {
            _namespaceConfigurationStore =  _services.GetRequiredService<INamespaceConfigurationStore>();

            return Task.CompletedTask;
        }

        [Test]
        public async Task GetLatestNamespaceConfiguration_()
        {
            // Arrange

            // Act

            // Assert
        }

        public override void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<INamespaceConfigurationStore, SqlNamespaceConfigurationStore>();
        }
    }
}
