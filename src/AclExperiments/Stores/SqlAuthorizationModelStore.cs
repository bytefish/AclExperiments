// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Database;
using AclExperiments.Database.Model;
using AclExperiments.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AclExperiments.Stores
{
    public class SqlAuthorizationModelStore : IAuthorizationModelStore
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public SqlAuthorizationModelStore(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<AuthorizationModel> GetAuthorizationModelAsync(string modelKey, CancellationToken cancellationToken)
        {
            using(var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                var authorizationModel = await context.AuthorizationModels
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ModelKey == modelKey, cancellationToken);

                if(authorizationModel == null)
                {
                    throw new InvalidOperationException($"No Authorization Model '{modelKey}' found");
                }

                try
                {
                    var deserializedAuthorizationModel = JsonSerializer.Deserialize<AuthorizationModel>(authorizationModel.Content);

                    if (deserializedAuthorizationModel == null)
                    {
                        throw new InvalidOperationException($"Failed to deserialize Authorization Model '{modelKey}'");
                    }

                    return deserializedAuthorizationModel;
                } 
                catch(Exception e)
                {
                    throw new InvalidOperationException($"Failed to deserialize the Authorization Model '{modelKey}'", e);
                }
            }
        }

        public async Task AddAuthorizationModelAsync(AuthorizationModel authorizationModel, CancellationToken cancellationToken)
        {
            using (var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                // Convert to Json
                var jsonSerializedAuthorizationModel = JsonSerializer.Serialize(authorizationModel, GetJsonSerializerOptions());

                // Convert to Sql Model
                var sqlAuthorizationModel = new SqlAuthorizationModel
                {
                    ModelKey = authorizationModel.ModelKey,
                    Name = authorizationModel.Name,
                    Description = authorizationModel.Description,
                    Content = jsonSerializedAuthorizationModel
                };


                await context.AuthorizationModels.AddAsync(sqlAuthorizationModel, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }


    }
}
