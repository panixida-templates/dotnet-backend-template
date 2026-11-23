using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Organization.Product.Module.Infrastructure.Persistence.Core;

using PANiXiDA.Core.Infrastructure.Storage.S3.Configuration;
using PANiXiDA.Core.Infrastructure.Storage.S3.DependencyInjection;

namespace Organization.Product.Module.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddPostgreSqlEfRepository<
            ModuleWriteDbContext, ModuleReadDbContext>(configuration);

        if (configuration.GetSection(S3StorageOptions.SectionName).Exists())
        {
            serviceCollection.AddS3FileStorage(configuration);
        }

        return serviceCollection;
    }
}
