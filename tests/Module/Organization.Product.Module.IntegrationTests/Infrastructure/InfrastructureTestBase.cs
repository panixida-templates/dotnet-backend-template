namespace Organization.Product.Module.IntegrationTests.Infrastructure;

[Collection(InfrastructureTestCollection.Name)]
public abstract class InfrastructureTestBase(InfrastructureTestFixture fixture) : IAsyncLifetime
{
    protected InfrastructureTestFixture Fixture { get; } = fixture;

    public async ValueTask InitializeAsync()
    {
        await Fixture.ResetDatabaseAsync(TestContext.Current.CancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }
}
