using Organization.Product.Module.Application.RepositoryExamples.Users.Write.Create;
using Organization.Product.Module.Domain.RepositoryExamples.Users;
using Organization.Product.Module.Domain.RepositoryExamples.Users.Abstractions;

namespace Organization.Product.Module.UnitTests.RepositoryExamples.Application.Users.Write.Create;

public sealed class CreateUserHandlerTests
{
    [Fact(DisplayName = "HandleAsync should create user and return id when command is valid")]
    public async Task HandleAsync_Should_Create_User_And_Return_Id_When_Command_Is_Valid()
    {
        var cancellationToken = CancellationToken.None;
        var usersRepository = Substitute.For<IUsersRepository>();
        User? addedUser = null;
        usersRepository
            .When(repository => repository.Add(Arg.Any<User>()))
            .Do(call => addedUser = call.Arg<User>());
        var handler = new CreateUserHandler(usersRepository);
        var command = new CreateUserCommand(
            Role: "Admin",
            Name: "John Doe",
            Email: "john@example.com",
            Phone: "+12345678901",
            BirthDate: DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-30),
            Avatar: "https://example.com/avatar.png");

        var result = await handler.HandleAsync(command, cancellationToken);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
        addedUser.ShouldNotBeNull();
        addedUser.Id.Value.ShouldBe(result.Value);
        addedUser.Role.Name.ShouldBe("Admin");
        addedUser.Email.Value.ShouldBe("john@example.com");
        usersRepository.Received(1).Add(Arg.Any<User>());
    }

    [Fact(DisplayName = "HandleAsync should return failure and not add user when command is invalid")]
    public async Task HandleAsync_Should_Return_Failure_And_Not_Add_User_When_Command_Is_Invalid()
    {
        var cancellationToken = CancellationToken.None;
        var usersRepository = Substitute.For<IUsersRepository>();
        var handler = new CreateUserHandler(usersRepository);
        var command = new CreateUserCommand(
            Role: "",
            Name: "",
            Email: "invalid-email",
            Phone: "123",
            BirthDate: DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-17),
            Avatar: new string('a', 2049));

        var result = await handler.HandleAsync(command, cancellationToken);

        result.IsFailure.ShouldBeTrue();
        result.Errors.Count.ShouldBe(6);
        usersRepository.DidNotReceive()
            .Add(Arg.Any<User>());
    }
}
