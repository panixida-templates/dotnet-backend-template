using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Organization.Product.Host.Common;

namespace Organization.Product.Module.FunctionalTests.Presentation;

public sealed class HostConfigurationTests(FunctionalTestFixture fixture)
    : FunctionalTestBase(fixture)
{
    [Fact(DisplayName = "Host should configure request body size when application starts")]
    public void Host_Should_ConfigureRequestBodySize_When_ApplicationStarts()
    {
        var options = Fixture.Services
            .GetRequiredService<IOptions<KestrelServerOptions>>()
            .Value;

        options.Limits.MaxRequestBodySize.ShouldBe(FilesConstants.FileRequestSizeLimit);
    }
}
