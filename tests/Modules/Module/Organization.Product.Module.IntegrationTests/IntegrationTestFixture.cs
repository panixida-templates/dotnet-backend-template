using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Organization.Product.Module.Infrastructure.DependencyInjection;
using Organization.Product.Module.Infrastructure.Persistence.Core;
using Organization.Product.Testing.Databases;

namespace Organization.Product.Module.IntegrationTests;

public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlTestDatabase _database = new();

    private ServiceProvider _serviceProvider = null!;

    public string ConnectionString => _database.PostgreSqlConnectionString;

    private static string PostgreSqlConnectionStringConfigurationKey =>
        PostgreSqlTestDatabase.PostgreSqlConnectionStringEnvironmentVariable.Replace(
            "__",
            ConfigurationPath.KeyDelimiter,
            StringComparison.Ordinal);

    public async ValueTask InitializeAsync()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await _database.InitializeAsync(cancellationToken);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [PostgreSqlConnectionStringConfigurationKey] = ConnectionString
            })
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        _serviceProvider = services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateScopes = true
            });

        await using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ModuleWriteDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    public AsyncServiceScope CreateScope()
    {
        return _serviceProvider.CreateAsyncScope();
    }

    public Task ResetDatabaseAsync(CancellationToken cancellationToken)
    {
        return _database.ResetPostgreSqlDatabaseAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_serviceProvider is not null)
        {
            await _serviceProvider.DisposeAsync();
        }

        await _database.DisposeAsync();
    }
}
