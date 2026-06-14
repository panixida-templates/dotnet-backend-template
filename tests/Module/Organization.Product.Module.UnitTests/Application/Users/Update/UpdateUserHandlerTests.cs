using Organization.Product.Module.Application.Users.Update;
using Organization.Product.Module.Domain.Users;
using Organization.Product.Module.Domain.Users.Abstractions;
using Organization.Product.Module.UnitTests.Domain;
using Organization.Product.Module.UnitTests.Domain.Users;

namespace Organization.Product.Module.UnitTests.Application.Users.Update;

public sealed class UpdateUserHandlerTests
{
    [Fact(DisplayName = "HandleAsync should return failure when id is empty")]
    public async Task HandleAsync_Should_Return_Failure_When_Id_Is_Empty()
    {
        var usersRepository = Substitute.For<IUsersRepository>();
        var handler = new UpdateUserHandler(usersRepository);
        var command = CreateCommand(Guid.Empty);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        result.ShouldHaveSingleError(
            ErrorType.Validation,
            "User id cannot be empty.");
        _ = usersRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());
        _ = usersRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "HandleAsync should return not found when user does not exist")]
    public async Task HandleAsync_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        var cancellationToken = CancellationToken.None;
        var userId = Guid.NewGuid();
        var usersRepository = Substitute.For<IUsersRepository>();
        usersRepository
            .GetByIdAsync(
                Arg.Is<UserId>(id => id.Value == userId),
                cancellationToken)
            .Returns(Task.FromResult<User?>(null));
        var handler = new UpdateUserHandler(usersRepository);

        var result = await handler.HandleAsync(CreateCommand(userId), cancellationToken);

        var error = result.ShouldHaveSingleError(
            ErrorType.NotFound,
            $"User with id '{userId}' was not found.");
        error.ShouldHaveField(nameof(User));
        _ = usersRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "HandleAsync should return failure and not update when command is invalid")]
    public async Task HandleAsync_Should_Return_Failure_And_Not_Update_When_Command_Is_Invalid()
    {
        var cancellationToken = CancellationToken.None;
        var user = UserTestFactory.CreateUser(
            name: "John Doe",
            email: "john@example.com",
            phone: "+12345678901",
            birthDate: UserTestFactory.AdultBirthDate(30));
        var usersRepository = Substitute.For<IUsersRepository>();
        usersRepository
            .GetByIdAsync(user.Id, cancellationToken)
            .Returns(Task.FromResult<User?>(user));
        var handler = new UpdateUserHandler(usersRepository);
        var command = new UpdateUserCommand(
            user.Id.Value,
            "",
            "",
            "invalid-email",
            "123",
            DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-17),
            new string('a', 2049));

        var result = await handler.HandleAsync(command, cancellationToken);

        result.IsFailure.ShouldBeTrue();
        result.Errors.Count.ShouldBe(6);
        user.Name.Value.ShouldBe("John Doe");
        user.Email.Value.ShouldBe("john@example.com");
        user.Phone.Value.ShouldBe("+12345678901");
        _ = usersRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "HandleAsync should update user when command is valid")]
    public async Task HandleAsync_Should_Update_User_When_Command_Is_Valid()
    {
        var cancellationToken = CancellationToken.None;
        var user = UserTestFactory.CreateUser(email: "old@example.com");
        var usersRepository = Substitute.For<IUsersRepository>();
        usersRepository
            .GetByIdAsync(user.Id, cancellationToken)
            .Returns(Task.FromResult<User?>(user));
        usersRepository
            .UpdateAsync(user, cancellationToken)
            .Returns(Task.CompletedTask);
        var handler = new UpdateUserHandler(usersRepository);
        var newBirthDate = UserTestFactory.AdultBirthDate(40);
        var command = new UpdateUserCommand(
            user.Id.Value,
            "Moderator",
            "Jane Doe",
            "new@example.com",
            "+19876543210",
            newBirthDate,
            null);

        var result = await handler.HandleAsync(command, cancellationToken);

        result.IsSuccess.ShouldBeTrue();
        user.Role.Name.ShouldBe("Moderator");
        user.Name.Value.ShouldBe("Jane Doe");
        user.Email.Value.ShouldBe("new@example.com");
        user.Phone.Value.ShouldBe("+19876543210");
        user.BirthDate.Value.ShouldBe(newBirthDate);
        user.Avatar.ShouldBeNull();
        _ = usersRepository.Received(1).UpdateAsync(user, cancellationToken);
    }

    private static UpdateUserCommand CreateCommand(Guid id)
    {
        return new UpdateUserCommand(
            id,
            "User",
            "John Doe",
            "john@example.com",
            "+12345678901",
            UserTestFactory.AdultBirthDate(),
            null);
    }
}
