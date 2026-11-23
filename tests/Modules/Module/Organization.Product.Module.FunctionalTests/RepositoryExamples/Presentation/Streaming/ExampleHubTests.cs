using System.Net;

using Organization.Product.Module.FunctionalTests.Presentation;

namespace Organization.Product.Module.FunctionalTests.RepositoryExamples.Presentation.Streaming;

[Collection(FunctionalTestCollection.Name)]
public sealed class ExampleHubTests
{
    [Fact(DisplayName = "Negotiate should respond successfully when SignalR is enabled")]
    public async Task Negotiate_Should_RespondSuccessfully_When_SignalRIsEnabled()
    {
        const string setting = "SignalR__Enabled";
        var previousValue = Environment.GetEnvironmentVariable(setting);

        try
        {
            Environment.SetEnvironmentVariable(setting, "true");
            await using var factory = new FunctionalTestWebApplicationFactory();
            using var client = factory.CreateClient();

            using var response = await client.PostAsync(
                "/hubs/example/negotiate?negotiateVersion=1",
                content: null,
                cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        finally
        {
            Environment.SetEnvironmentVariable(setting, previousValue);
        }
    }
}
