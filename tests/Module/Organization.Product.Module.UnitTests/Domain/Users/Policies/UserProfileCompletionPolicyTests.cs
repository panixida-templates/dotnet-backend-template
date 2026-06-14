using Organization.Product.Module.Domain.Users.Policies;

namespace Organization.Product.Module.UnitTests.Domain.Users.Policies;

public sealed class UserProfileCompletionPolicyTests
{
    [Fact(DisplayName = "EnsureCompleted should return success when user is regular and avatar is not provided")]
    public void EnsureCompleted_Should_Return_Success_When_User_Is_Regular_And_Avatar_Is_Not_Provided()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var user = UserTestFactory.CreateUser(
            role: "User",
            birthDate: today.AddYears(-20),
            avatar: null);

        var result = UserProfileCompletionPolicy.EnsureCompleted(user, today);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(DisplayName = "EnsureCompleted should return failure when privileged user has no avatar")]
    public void EnsureCompleted_Should_Return_Failure_When_Privileged_User_Has_No_Avatar()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var user = UserTestFactory.CreateUser(
            role: "Admin",
            birthDate: today.AddYears(-30),
            avatar: null);

        var result = UserProfileCompletionPolicy.EnsureCompleted(user, today);

        var error = result.ShouldHaveSingleError(
            ErrorType.Validation,
            "Privileged users must have an avatar.");

        error.ShouldHaveField("Avatar");
    }

    [Fact(DisplayName = "EnsureCompleted should return failure when privileged user is under 21")]
    public void EnsureCompleted_Should_Return_Failure_When_Privileged_User_Is_Under_21()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var user = UserTestFactory.CreateUser(
            role: "Moderator",
            birthDate: today.AddYears(-20),
            avatar: "https://example.com/avatar.png");

        var result = UserProfileCompletionPolicy.EnsureCompleted(user, today);

        var error = result.ShouldHaveSingleError(
            ErrorType.Validation,
            "Privileged users must be at least 21 years old.");

        error.ShouldHaveField("BirthDate");
    }

    [Fact(DisplayName = "EnsureCompleted should return success when privileged user has avatar and is at least 21")]
    public void EnsureCompleted_Should_Return_Success_When_Privileged_User_Has_Avatar_And_Is_At_Least_21()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var user = UserTestFactory.CreateUser(
            role: "Admin",
            birthDate: today.AddYears(-21),
            avatar: "https://example.com/avatar.png");

        var result = UserProfileCompletionPolicy.EnsureCompleted(user, today);

        result.IsSuccess.ShouldBeTrue();
    }
}
