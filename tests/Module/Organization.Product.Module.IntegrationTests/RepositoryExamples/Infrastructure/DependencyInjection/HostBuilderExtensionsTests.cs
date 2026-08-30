using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Organization.Product.Module.Infrastructure.DependencyInjection;

namespace Organization.Product.Module.IntegrationTests.RepositoryExamples.Infrastructure.DependencyInjection;

public sealed class HostBuilderExtensionsTests
{
    [Fact(DisplayName = "UseInfrastructure should return host builder when configuration is valid")]
    public void UseInfrastructure_Should_Return_Host_Builder_When_Configuration_Is_Valid()
    {
        var hostBuilder = new HostBuilder();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"ConnectionStrings:{EfConstants.PostgreSqlConnectionStringName}"] = "Host=localhost"
            })
            .Build();

        var result = hostBuilder.UseInfrastructure(configuration);

        result.ShouldBeSameAs(hostBuilder);
    }

    [Fact(DisplayName = "UseInfrastructure should fail when PostgreSQL connection string is missing")]
    public void UseInfrastructure_Should_Fail_When_PostgreSql_Connection_String_Is_Missing()
    {
        var hostBuilder = new HostBuilder();
        var configuration = new ConfigurationBuilder().Build();

        var action = () => hostBuilder.UseInfrastructure(configuration);

        var exception = action.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe(
            $"Connection string '{EfConstants.PostgreSqlConnectionStringName}' was not found.");
    }
}
