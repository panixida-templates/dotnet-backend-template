using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Organization.Product.Module.Infrastructure.Persistence.Core;
using Organization.Product.Testing.Databases;

namespace Organization.Product.Module.FunctionalTests.Presentation;

public sealed class FunctionalTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlTestDatabase _database = new();

    private FunctionalTestWebApplicationFactory _factory = null!;

    public HttpClient Client { get; private set; } = null!;
    public IServiceProvider Services => _factory.Services;

    public async ValueTask InitializeAsync()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await _database.InitializeAsync(cancellationToken);

        Environment.SetEnvironmentVariable(
            PostgreSqlTestDatabase.PostgreSqlConnectionStringEnvironmentVariable,
            _database.PostgreSqlConnectionString);

        _factory = new FunctionalTestWebApplicationFactory();
        Client = _factory.CreateClient();

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ModuleWriteDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    public Task ResetDatabaseAsync(CancellationToken cancellationToken)
    {
        return _database.ResetPostgreSqlDatabaseAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        Client?.Dispose();
        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }

        await _database.DisposeAsync();
    }
}
