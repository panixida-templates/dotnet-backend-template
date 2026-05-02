using Infrastructure.Ef.Core.Bootstrapper;
using Infrastructure.Ef.EfCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Ef.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddPostgreSqlEfRepository<TemplateWriteDbContext, TemplateReadDbContext>(configuration);

        return serviceCollection;
    }
}
