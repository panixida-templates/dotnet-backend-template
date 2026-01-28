using Application.Abstractions.Persistence.Core;

using Common.Constants;

using Infrastructure.Persistence.Ef.Core;
using Infrastructure.Persistence.Ef.EfCore;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Infrastructure.Persistence.Ef.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEfDal(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<DefaultDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString(AppsettingsKeysConstants.PostgreSqlConnectionString)).UseSnakeCaseNamingConvention());

        serviceCollection.RegisterDalImplementations(typeof(EfRepository<,,,,,,,,>).Assembly);

        return serviceCollection;
    }

    private static IServiceCollection RegisterDalImplementations(this IServiceCollection serviceCollection, Assembly assembly)
    {
        foreach (var dalImplementation in assembly.GetTypes())
        {
            if (!dalImplementation.IsClass || dalImplementation.IsAbstract || dalImplementation.IsGenericTypeDefinition)
            {
                continue;
            }

            var dalInterfaces = dalImplementation.GetInterfaces()
                .Where(iface =>
                {
                    if (!iface.IsInterface || iface.IsGenericType)
                    {
                        return false;
                    }

                    return iface.GetInterfaces().Any(parent =>
                    {
                        return parent.IsGenericType && parent.GetGenericTypeDefinition() == typeof(IRepository<,,,>);
                    });
                })
                .ToArray();

            foreach (var dalInterface in dalInterfaces)
            {
                if (serviceCollection.Any(descriptor =>
                {
                    return descriptor.ServiceType == dalInterface;
                }))
                {
                    throw new InvalidOperationException(
                        $"DAL интерфейс '{dalInterface.FullName}' уже зарегистрирован. " +
                        $"Конфликт реализации: '{dalImplementation.FullName}'.");
                }

                serviceCollection.AddScoped(dalInterface, dalImplementation);
            }
        }

        return serviceCollection;
    }
}
