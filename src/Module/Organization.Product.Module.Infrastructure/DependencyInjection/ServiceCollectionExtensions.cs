using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Organization.Product.Module.Application.Users.Create;
using Organization.Product.Module.Infrastructure.EfCore;

using PANiXiDA.Core.Infrastructure.Messaging.Wolverine.DependencyInjection;
using PANiXiDA.Core.Infrastructure.Persistence.Ef.Constants;
using PANiXiDA.Core.Infrastructure.Persistence.Ef.DependencyInjection;

namespace Organization.Product.Module.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddPostgreSqlEfRepository<
            TemplateWriteDbContext, TemplateReadDbContext>(configuration);

        serviceCollection.AddWolverineMediator<TemplateWriteDbContext>();

        return serviceCollection;
    }

    public static IHostBuilder UseInfrastructure(
        this IHostBuilder hostBuilder,
        IConfiguration configuration)
    {
        var messageStoreConnectionString =
            configuration.GetConnectionString(EfConstants.PostgreSqlConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{EfConstants.PostgreSqlConnectionStringName}' was not found.");

        hostBuilder.UseWolverineMediator<TemplateWriteDbContext>(
            messageStoreConnectionString,
            typeof(CreateUserHandler).Assembly);

        return hostBuilder;
    }
}
