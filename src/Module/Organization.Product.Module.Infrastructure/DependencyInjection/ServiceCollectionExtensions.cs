using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Organization.Product.Module.Application;
using Organization.Product.Module.Domain;
using Organization.Product.Module.Infrastructure.Persistence.Core;

namespace Organization.Product.Module.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        _ = ApplicationAssembly.Instance;
        _ = DomainAssembly.Instance;

        serviceCollection.AddPostgreSqlEfRepository<
            TemplateWriteDbContext, TemplateReadDbContext>(configuration);

        serviceCollection.AddWolverineMediator<TemplateWriteDbContext>();

        return serviceCollection;
    }
}
