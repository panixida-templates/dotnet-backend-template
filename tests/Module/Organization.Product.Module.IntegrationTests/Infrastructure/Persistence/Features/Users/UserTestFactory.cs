using Microsoft.Extensions.DependencyInjection;

using Organization.Product.Module.Domain.Users;
using Organization.Product.Module.Domain.Users.Abstractions;

namespace Organization.Product.Module.IntegrationTests.Infrastructure.Persistence.Features.Users;

internal static class UserTestFactory
{
    public static User CreateUser(
        string role = "User",
        string name = "John Doe",
        string email = "john@example.com",
        string phone = "+12345678901",
        DateOnly? birthDate = null,
        string? avatar = "https://example.com/avatar.png")
    {
        var result = User.Create(
            role,
            name,
            email,
            phone,
            birthDate ?? AdultBirthDate(),
            avatar);

        result.IsSuccess.ShouldBeTrue();

        return result.Value;
    }

    public static DateOnly AdultBirthDate(int age = 30)
    {
        return DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-age);
    }

    public static async Task AddUsersAsync(
        InfrastructureTestFixture fixture,
        CancellationToken cancellationToken,
        params User[] users)
    {
        await using var scope = fixture.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                foreach (var user in users)
                {
                    await repository.AddAsync(user, ct);
                }
            },
            cancellationToken);
    }
}
