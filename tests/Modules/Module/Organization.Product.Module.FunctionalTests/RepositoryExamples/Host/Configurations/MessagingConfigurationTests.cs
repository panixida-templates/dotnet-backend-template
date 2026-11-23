using Microsoft.AspNetCore.Builder;

using Organization.Product.Host.Configurations;

using PANiXiDA.Core.Infrastructure.Persistence.Ef.Constants;

namespace Organization.Product.Module.FunctionalTests.RepositoryExamples.Host.Configurations;

public sealed class MessagingConfigurationTests
{
    [Fact(DisplayName = "AddMessaging should return the builder when PostgreSQL is configured")]
    public void AddMessaging_Should_ReturnApplicationBuilder_When_PostgreSqlConnectionIsConfigured()
    {
        var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
        builder.Configuration[$"ConnectionStrings:{EfConstants.PostgreSqlConnectionStringName}"] = "Host=localhost";

        var result = builder.AddMessaging();

        result.ShouldBeSameAs(builder);
    }

    [Fact(DisplayName = "AddMessaging should fail when the PostgreSQL connection string is missing")]
    public void AddMessaging_Should_Fail_When_PostgreSqlConnectionStringIsMissing()
    {
        var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
        builder.Configuration.Sources.Clear();

        var action = () => builder.AddMessaging();

        var exception = action.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe(
            $"Connection string '{EfConstants.PostgreSqlConnectionStringName}' was not found.");
    }
}
