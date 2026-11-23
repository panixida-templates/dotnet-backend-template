using System.Text.Json;

namespace Organization.Product.Module.FunctionalTests.Presentation;

[Collection(FunctionalTestCollectionDefinition.Name)]
public abstract class FunctionalTestBase(FunctionalTestFixture fixture) : IAsyncLifetime
{
    protected static JsonSerializerOptions JsonOptions => TestJsonSerializerOptions.Web;

    protected FunctionalTestFixture Fixture { get; } = fixture;

    protected HttpClient Client => Fixture.Client;

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
