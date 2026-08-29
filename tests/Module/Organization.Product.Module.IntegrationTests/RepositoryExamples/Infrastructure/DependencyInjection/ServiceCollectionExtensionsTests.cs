using Microsoft.Extensions.DependencyInjection;

using Organization.Product.Module.Application.RepositoryExamples.Users.Abstractions;
using Organization.Product.Module.Domain.RepositoryExamples.Users.Abstractions;
using Organization.Product.Module.Infrastructure.Persistence.Core;

namespace Organization.Product.Module.IntegrationTests.RepositoryExamples.Infrastructure.DependencyInjection;

public sealed class ServiceCollectionExtensionsTests(IntegrationTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact(DisplayName = "AddInfrastructure should resolve persistence services when configuration is valid")]
    public async Task AddInfrastructure_Should_Resolve_Persistence_Services_When_Configuration_Is_Valid()
    {
        await using var scope = Fixture.CreateScope();

        var writeDbContext = scope.ServiceProvider.GetRequiredService<TemplateWriteDbContext>();
        var readDbContext = scope.ServiceProvider.GetRequiredService<TemplateReadDbContext>();
        var usersRepository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
        var usersReadRepository = scope.ServiceProvider.GetRequiredService<IUsersReadRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredKeyedService<IUnitOfWork>(
            typeof(TemplateWriteDbContext));

        writeDbContext.ShouldNotBeNull();
        readDbContext.ShouldNotBeNull();
        usersRepository.ShouldNotBeNull();
        usersReadRepository.ShouldNotBeNull();
        unitOfWork.ShouldNotBeNull();
    }
}
