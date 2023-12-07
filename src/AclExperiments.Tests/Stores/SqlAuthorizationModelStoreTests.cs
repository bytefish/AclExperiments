using AclExperiments.Database;
using AclExperiments.Database.Model;
using AclExperiments.Stores;
using AclExperiments.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AclExperiments.Tests.Stores
{
    [TestClass]
    public class SqlAuthorizationModelStoreTests : IntegrationTestBase
    {
        private IAuthorizationModelStore _authorizationModelStore = null!;

        protected override Task OnSetupBeforeCleanupAsync()
        {
            _dbContextFactory = _services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            _authorizationModelStore = _services.GetRequiredService<IAuthorizationModelStore>();

            return Task.CompletedTask;
        }

        [TestMethod]
        public async Task GetAuthorizationModel_ReturnsDeserializedAuthorizationModel()
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

            // Act
            var result = await _authorizationModelStore.GetAuthorizationModelAsync("google-drive", default);

            // Assert
            Assert.AreEqual(4, result.Namespaces.Count);
        }

        public override void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationModelStore, SqlAuthorizationModelStore>();
        }
    }
}
