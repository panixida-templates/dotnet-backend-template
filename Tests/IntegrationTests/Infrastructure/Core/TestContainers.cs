using Common.Constants;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Infrastructure.Core;

public sealed class TestContainers(IConfiguration configuration)
{
    public PostgresContainer? PostgresContainer { get; private set; }
    public MongoContainer? MongoContainer { get; private set; }

    private readonly bool _useEf = configuration.GetValue<bool>(AppsettingsKeysConstants.DalUseEf);
    private readonly bool _useMongo = configuration.GetValue<bool>(AppsettingsKeysConstants.DalUseMongo);

    public async Task InitializeAsync()
    {
        if (_useEf)
        {
            PostgresContainer = new PostgresContainer();
            await PostgresContainer.InitializeAsync();
        }
        if (_useMongo)
        {
            MongoContainer = new MongoContainer();
            await MongoContainer.InitializeAsync();
        }
    }

    public async Task DisposeAsync()
    {
        if (PostgresContainer is not null)
        {
            await PostgresContainer.DisposeAsync();
        }
        if (MongoContainer is not null)
        {
            await MongoContainer.DisposeAsync();
        }
    }

    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton(this);

        if (PostgresContainer is not null)
        {
            services.AddSingleton(PostgresContainer);
        }
        if (MongoContainer is not null)
        {
            services.AddSingleton(MongoContainer);
        }
    }

    public async Task ResetDatabaseAsync()
    {
        if (PostgresContainer is not null)
        {
            await PostgresContainer.ResetDatabaseAsync();
        }
        if (MongoContainer is not null)
        {
            await MongoContainer.ResetDatabaseAsync();
        }
    }

    public IDictionary<string, string?> BuildAppsettingsOverrides()
    {
        var appsettingsDictionary = new Dictionary<string, string?>();

        if (PostgresContainer is not null)
        {
            appsettingsDictionary[AppsettingsKeysConstants.ConnectionStringsPostgreSqlConnectionString] = PostgresContainer.ConnectionString;
        }
        if (MongoContainer is not null)
        {
            appsettingsDictionary[AppsettingsKeysConstants.ConnectionStringsMongoDbConnectionString] = MongoContainer.ConnectionString;
        }

        return appsettingsDictionary;
    }
}
