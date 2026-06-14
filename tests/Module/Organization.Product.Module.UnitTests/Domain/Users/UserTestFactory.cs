using Organization.Product.Module.Domain.Users;

namespace Organization.Product.Module.UnitTests.Domain.Users;

internal static class UserTestFactory
{
    public static DateOnly AdultBirthDate(int age = 30)
    {
        return DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-age);
    }

    public static User CreateUser(
        string role = "User",
        string name = "John Doe",
        string email = "john.doe@example.com",
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

        return result.Value;
    }
}
