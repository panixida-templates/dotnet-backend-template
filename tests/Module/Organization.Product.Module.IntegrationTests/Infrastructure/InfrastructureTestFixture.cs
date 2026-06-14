using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using Organization.Product.Module.Infrastructure.DependencyInjection;
using Organization.Product.Module.Infrastructure.Persistence.Core;

using Respawn;
using Respawn.Graph;

using Testcontainers.PostgreSql;

namespace Organization.Product.Module.IntegrationTests.Infrastructure;

public sealed class InfrastructureTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("organization_product_module_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private Respawner _respawner = null!;
    private ServiceProvider _serviceProvider = null!;

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"ConnectionStrings:{EfConstants.PostgreSqlConnectionStringName}"] = ConnectionString
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
        var dbContext = scope.ServiceProvider.GetRequiredService<TemplateWriteDbContext>();
        await dbContext.Database.MigrateAsync();

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                TablesToIgnore =
                [
                    new Table("__EFMigrationsHistory")
                ]
            });
    }

    public AsyncServiceScope CreateScope()
    {
        return _serviceProvider.CreateAsyncScope();
    }

    public async Task ResetDatabaseAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await _respawner.ResetAsync(connection);
    }

    public async ValueTask DisposeAsync()
    {
        if (_serviceProvider is not null)
        {
            await _serviceProvider.DisposeAsync();
        }

        await _container.DisposeAsync();
    }
}
