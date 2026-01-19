using Common.Constants;

using Dal.Ef.Implementations.Core;
using Dal.Interfaces.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Dal.Ef.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection UseEfDal(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<DefaultDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString(AppsettingsKeysConstants.PostgreSqlConnectionString)).UseSnakeCaseNamingConvention());

        serviceCollection.RegisterDalImplementations(typeof(BaseDal<,,,,,,,,>).Assembly);

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
                        return parent.IsGenericType && parent.GetGenericTypeDefinition() == typeof(IBaseDal<,,,>);
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
