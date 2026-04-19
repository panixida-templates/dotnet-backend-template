using Application.Abstractions.Persistence;
using Application.Abstractions.Queries;

using Infrastructure.Persistence.Ef.Core;
using Infrastructure.Persistence.Ef.EfCore;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Infrastructure.Persistence.Ef.DependencyInjection;

public static class ServiceCollectionExtensions
{
    internal const string PostgreSqlConnectionString = "PostgreSqlConnectionString";

    public static IServiceCollection AddEfRepository(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<DefaultDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString(PostgreSqlConnectionString)).UseSnakeCaseNamingConvention());

        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork<DefaultDbContext>>();
        serviceCollection.AddScoped<IAggregateTracker, AggregateTracker>();
        serviceCollection.RegisterRepositoryImplementations(typeof(Repository<,,,,>).Assembly);

        return serviceCollection;
    }

    private static IServiceCollection RegisterRepositoryImplementations(this IServiceCollection serviceCollection, Assembly assembly)
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
                        return parent.IsGenericType
                            && (parent.GetGenericTypeDefinition() == typeof(IRepository<,>)
                                || parent.GetGenericTypeDefinition() == typeof(IQueryService<,,,>));
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
