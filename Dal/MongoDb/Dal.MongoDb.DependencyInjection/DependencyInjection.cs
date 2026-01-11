using Common.Constants;
using Dal.Interfaces;
using Dal.MongoDb.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Dal.MongoDb.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection UseMongoDal(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(new NullableSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard)));

        serviceCollection.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = configuration.GetConnectionString(AppsettingsKeysConstants.MongoDbConnectionString);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("MongoDb connection string is not configured (ConnectionStrings:MongoDb).");
            }

            return new MongoClient(connectionString);
        });

        serviceCollection.AddScoped(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();

            var databaseName = configuration[$"{AppsettingsKeysConstants.MongoDbDatabase}"];

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new InvalidOperationException(
                    $"MongoDb database is not configured ({AppsettingsKeysConstants.MongoDbDatabase}).");
            }

            var client = sp.GetRequiredService<IMongoClient>();

            return client.GetDatabase(databaseName);
        });

        serviceCollection.AddScoped<IUsersDal, UsersDal>();

        return serviceCollection;
    }
}
