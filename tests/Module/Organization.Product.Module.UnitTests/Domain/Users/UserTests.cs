using Organization.Product.Module.Domain.Users;
using Organization.Product.Module.Domain.Users.Enumerations;
using Organization.Product.Module.Domain.Users.Events;

namespace Organization.Product.Module.UnitTests.Domain.Users;

public sealed class UserTests
{
    [Fact(DisplayName = "Create should create user with normalized values when input is valid")]
    public void Create_Should_Create_User_With_Normalized_Values_When_Input_Is_Valid()
    {
        var birthDate = UserTestFactory.AdultBirthDate();

        var result = User.Create(
            " Admin ",
            "  John Doe  ",
            "  JOHN.DOE@EXAMPLE.COM  ",
            "  +1 (234) 567-8901  ",
            birthDate,
            "  https://example.com/avatar.png  ");

        result.IsSuccess.ShouldBeTrue();

        var user = result.Value;
        user.Id.Value.ShouldNotBe(Guid.Empty);
        user.Role.ShouldBe(UserRole.Admin);
        user.Name.Value.ShouldBe("John Doe");
        user.Email.Value.ShouldBe("john.doe@example.com");
        user.Phone.Value.ShouldBe("+12345678901");
        user.BirthDate.Value.ShouldBe(birthDate);
        user.Avatar.ShouldNotBeNull();
        user.Avatar.Value.ShouldBe("https://example.com/avatar.png");
        user.GetDomainEvents().ShouldBeEmpty();
    }

    [Fact(DisplayName = "Create should return all validation errors when input is invalid")]
    public void Create_Should_Return_All_Validation_Errors_When_Input_Is_Invalid()
    {
        var result = User.Create(
            "",
            "",
            "",
            "",
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1),
            new string('a', 2049));

        result.IsFailure.ShouldBeTrue();
        result.Errors.Count.ShouldBe(6);
        result.Errors.ShouldContain(error => error.Message == "User role cannot be empty.");
        result.Errors.ShouldContain(error => error.Message == "User name cannot be empty.");
        result.Errors.ShouldContain(error => error.Message == "Email cannot be empty.");
        result.Errors.ShouldContain(error => error.Message == "Phone number cannot be empty.");
        result.Errors.ShouldContain(error => error.Message == "Birth date cannot be in the future.");
        result.Errors.ShouldContain(error => error.Message == "Avatar cannot be longer than 2048 characters.");
    }

    [Fact(DisplayName = "Update should update user and raise email changed event when email changes")]
    public void Update_Should_Update_User_And_Raise_Email_Changed_Event_When_Email_Changes()
    {
        var user = UserTestFactory.CreateUser(
            role: "User",
            email: "old@example.com",
            avatar: null);
        var newBirthDate = UserTestFactory.AdultBirthDate(35);

        var result = user.Update(
            "Admin",
            "Jane Doe",
            "new@example.com",
            "+19876543210",
            newBirthDate,
            "https://example.com/new-avatar.png");

        result.IsSuccess.ShouldBeTrue();
        user.Role.ShouldBe(UserRole.Admin);
        user.Name.Value.ShouldBe("Jane Doe");
        user.Email.Value.ShouldBe("new@example.com");
        user.Phone.Value.ShouldBe("+19876543210");
        user.BirthDate.Value.ShouldBe(newBirthDate);
        user.Avatar.ShouldNotBeNull();
        user.Avatar.Value.ShouldBe("https://example.com/new-avatar.png");

        var domainEvent = user.GetDomainEvents().ShouldHaveSingleItem();
        var emailChanged = domainEvent.ShouldBeOfType<UserEmailChanged>();
        emailChanged.UserId.ShouldBe(user.Id.Value);
        emailChanged.OldEmail.ShouldBe("old@example.com");
        emailChanged.NewEmail.ShouldBe("new@example.com");
    }

    [Fact(DisplayName = "Update should not raise email changed event when email is not changed")]
    public void Update_Should_Not_Raise_Email_Changed_Event_When_Email_Is_Not_Changed()
    {
        var user = UserTestFactory.CreateUser(email: "same@example.com");

        var result = user.Update(
            "Moderator",
            "Jane Doe",
            "same@example.com",
            "+19876543210",
            UserTestFactory.AdultBirthDate(40),
            null);

        result.IsSuccess.ShouldBeTrue();
        user.Email.Value.ShouldBe("same@example.com");
        user.Avatar.ShouldBeNull();
        user.GetDomainEvents().ShouldBeEmpty();
    }

    [Fact(DisplayName = "Update should return failure and keep current state when input is invalid")]
    public void Update_Should_Return_Failure_And_Keep_Current_State_When_Input_Is_Invalid()
    {
        var user = UserTestFactory.CreateUser(
            role: "User",
            name: "John Doe",
            email: "john@example.com",
            phone: "+12345678901",
            birthDate: UserTestFactory.AdultBirthDate(30),
            avatar: "https://example.com/avatar.png");

        var result = user.Update(
            "",
            "",
            "invalid-email",
            "123",
            DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-17),
            new string('a', 2049));

        result.IsFailure.ShouldBeTrue();
        result.Errors.Count.ShouldBe(6);
        user.Role.ShouldBe(UserRole.User);
        user.Name.Value.ShouldBe("John Doe");
        user.Email.Value.ShouldBe("john@example.com");
        user.Phone.Value.ShouldBe("+12345678901");
        user.BirthDate.Value.ShouldBe(UserTestFactory.AdultBirthDate(30));
        user.Avatar.ShouldNotBeNull();
        user.Avatar.Value.ShouldBe("https://example.com/avatar.png");
        user.GetDomainEvents().ShouldBeEmpty();
    }

    [Fact(DisplayName = "ClearDomainEvents should remove collected domain events when events exist")]
    public void ClearDomainEvents_Should_Remove_Collected_Domain_Events_When_Events_Exist()
    {
        var user = UserTestFactory.CreateUser(email: "old@example.com");
        var updateResult = user.Update(
            "User",
            "John Doe",
            "new@example.com",
            "+12345678901",
            UserTestFactory.AdultBirthDate(),
            "https://example.com/avatar.png");

        updateResult.IsSuccess.ShouldBeTrue();
        user.GetDomainEvents().ShouldNotBeEmpty();

        user.ClearDomainEvents();

        user.GetDomainEvents().ShouldBeEmpty();
    }
}
