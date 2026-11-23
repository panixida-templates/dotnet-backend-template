using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Organization.Product.Module.Infrastructure.DependencyInjection;

using PANiXiDA.Core.Application.Storage;

namespace Organization.Product.Module.IntegrationTests.Infrastructure.DependencyInjection;

public sealed class FileStorageRegistrationTests
{
    [Theory(DisplayName = "File storage should be registered conditionally when infrastructure is configured")]
    [InlineData(false)]
    [InlineData(true)]
    public void FileStorage_Should_BeRegisteredConditionally_When_InfrastructureIsConfigured(bool configureStorage)
    {
        var settings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:PostgreSqlConnectionString"] = "Host=localhost;Database=integration-tests"
        };

        if (configureStorage)
        {
            settings["S3Storage:BucketName"] = "integration-tests";
        }

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        var services = new ServiceCollection().AddInfrastructure(configuration);

        Assert.Equal(configureStorage, services.Any(service => service.ServiceType == typeof(IFileStorage)));
    }
}
