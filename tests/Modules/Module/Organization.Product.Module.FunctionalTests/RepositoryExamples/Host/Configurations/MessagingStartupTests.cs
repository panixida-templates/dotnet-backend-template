using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using PANiXiDA.Core.Infrastructure.Persistence.Ef.Constants;

namespace Organization.Product.Module.FunctionalTests.RepositoryExamples.Host.Configurations;

public sealed class MessagingStartupTests
{
    [Fact(DisplayName = "Host should fail to start when the PostgreSQL connection string is empty")]
    public void Host_Should_FailToStart_When_PostgreSqlConnectionStringIsEmpty()
    {
        using var factory = new EmptyPostgreSqlConnectionFactory();

        var action = () => factory.CreateClient();

        var exception = action.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe(
            $"Connection string '{EfConstants.PostgreSqlConnectionStringName}' was not found.");
    }

    private sealed class EmptyPostgreSqlConnectionFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureHostConfiguration(
                configuration =>
                {
                    configuration.AddInMemoryCollection(
                        [new($"ConnectionStrings:{EfConstants.PostgreSqlConnectionStringName}", string.Empty)]);
                });

            return base.CreateHost(builder);
        }
    }
}
