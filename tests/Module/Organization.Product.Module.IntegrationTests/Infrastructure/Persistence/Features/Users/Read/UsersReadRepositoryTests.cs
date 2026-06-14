using Microsoft.Extensions.DependencyInjection;

using Organization.Product.Module.Application.Users;
using Organization.Product.Module.Application.Users.Abstractions;

namespace Organization.Product.Module.IntegrationTests.Infrastructure.Persistence.Features.Users.Read;

public sealed class UsersReadRepositoryTests(InfrastructureTestFixture fixture)
    : InfrastructureTestBase(fixture)
{
    [Fact(DisplayName = "GetByIdAsync should return user details when user exists")]
    public async Task GetByIdAsync_Should_Return_User_Details_When_User_Exists()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var user = UserTestFactory.CreateUser(
            role: "Admin",
            name: "John Doe",
            email: "john@example.com",
            phone: "+12345678901",
            birthDate: UserTestFactory.AdultBirthDate(35));
        await UserTestFactory.AddUsersAsync(Fixture, cancellationToken, user);

        await using var scope = Fixture.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUsersReadRepository>();

        var readModel = await repository.GetByIdAsync(user.Id.Value, cancellationToken);

        readModel.ShouldNotBeNull();
        readModel.Id.ShouldBe(user.Id.Value);
        readModel.Name.ShouldBe("John Doe");
        readModel.Email.ShouldBe("john@example.com");
        readModel.Phone.ShouldBe("+12345678901");
        readModel.BirthDate.ShouldBe(UserTestFactory.AdultBirthDate(35));
        readModel.Avatar.ShouldBe("https://example.com/avatar.png");
    }

    [Fact(DisplayName = "GetPagedListAsync should filter and sort users when role filter is provided")]
    public async Task GetPagedListAsync_Should_Filter_And_Sort_Users_When_Role_Filter_Is_Provided()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var firstAdmin = UserTestFactory.CreateUser(
            role: "Admin",
            name: "Charlie Admin",
            email: "charlie@example.com",
            phone: "+12345678901");
        var secondAdmin = UserTestFactory.CreateUser(
            role: "Admin",
            name: "Alice Admin",
            email: "alice@example.com",
            phone: "+12345678902");
        var regularUser = UserTestFactory.CreateUser(
            role: "User",
            name: "Bob User",
            email: "bob@example.com",
            phone: "+12345678903");
        await UserTestFactory.AddUsersAsync(
            Fixture,
            cancellationToken,
            firstAdmin,
            secondAdmin,
            regularUser);

        await using var scope = Fixture.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUsersReadRepository>();
        var filterParameters = new UsersFilterParameters("admin");
        var paginationParameters = new PaginationParameters(1, 10);
        var sortParameters = new SortParameters("Name", SortOrder.Ascending);

        var result = await repository.GetPagedListAsync(
            filterParameters,
            paginationParameters,
            sortParameters,
            cancellationToken);

        result.TotalCount.ShouldBe(2);
        result.Items.Select(item => item.Name).ShouldBe(["Alice Admin", "Charlie Admin"]);
        result.Items.ShouldAllBe(item => item.Email.EndsWith("@example.com"));
    }

    [Fact(DisplayName = "GetPagedListAsync should return requested page when users exist")]
    public async Task GetPagedListAsync_Should_Return_Requested_Page_When_Users_Exist()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var firstUser = UserTestFactory.CreateUser(
            name: "Alice User",
            email: "alice@example.com",
            phone: "+12345678901");
        var secondUser = UserTestFactory.CreateUser(
            name: "Bob User",
            email: "bob@example.com",
            phone: "+12345678902");
        var thirdUser = UserTestFactory.CreateUser(
            name: "Charlie User",
            email: "charlie@example.com",
            phone: "+12345678903");
        await UserTestFactory.AddUsersAsync(
            Fixture,
            cancellationToken,
            firstUser,
            secondUser,
            thirdUser);

        await using var scope = Fixture.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUsersReadRepository>();
        var filterParameters = new UsersFilterParameters(null);
        var paginationParameters = new PaginationParameters(2, 1);
        var sortParameters = new SortParameters("Name", SortOrder.Ascending);

        var result = await repository.GetPagedListAsync(
            filterParameters,
            paginationParameters,
            sortParameters,
            cancellationToken);

        result.PageNumber.ShouldBe(2);
        result.PageSize.ShouldBe(1);
        result.TotalCount.ShouldBe(3);
        result.TotalPages.ShouldBe(3);
        result.Items.ShouldHaveSingleItem().Name.ShouldBe("Bob User");
    }
}
