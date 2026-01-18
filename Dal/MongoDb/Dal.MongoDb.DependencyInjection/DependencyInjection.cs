using Common.Constants;

using Dal.Interfaces.Core;

using Dal.MongoDb.Implementations.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

using System.Reflection;

namespace Dal.MongoDb.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection UseMongoDal(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(new NullableSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard)));
        BsonSerializer.RegisterIdGenerator(typeof(Guid), GuidGenerator.Instance);

        serviceCollection.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var connectionString = configuration.GetConnectionString(AppsettingsKeysConstants.MongoDbConnectionString);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("MongoDb connection string is not configured (ConnectionStrings:MongoDb).");
            }

            return new MongoClient(connectionString);
        });

        serviceCollection.AddScoped(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var databaseName = configuration[$"{AppsettingsKeysConstants.MongoDbDatabase}"];

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new InvalidOperationException($"MongoDb database is not configured ({AppsettingsKeysConstants.MongoDbDatabase}).");
            }

            var client = serviceProvider.GetRequiredService<IMongoClient>();

            return client.GetDatabase(databaseName);
        });

        serviceCollection.RegisterDalImplementations(typeof(BaseDal<,,,,,,>).Assembly);

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
