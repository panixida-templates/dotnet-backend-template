using System.Net;

namespace Organization.Product.Module.FunctionalTests.Presentation.Host;

public sealed class HostSmokeTests(FunctionalTestFixture fixture)
    : FunctionalTestBase(fixture)
{
    [Fact(DisplayName = "Host should start and return not found for unknown route")]
    public async Task Host_Should_Start_And_Return_NotFound_For_Unknown_Route()
    {
        using var response = await Client.GetAsync(
            "/__unknown",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
