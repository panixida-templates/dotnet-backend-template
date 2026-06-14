using Organization.Product.Module.Domain.Users.Enumerations;
using Organization.Product.Module.Domain.Users.Policies;

namespace Organization.Product.Module.UnitTests.Domain.Users.Policies;

public sealed class UserRoleAssignmentPolicyTests
{
    [Fact(DisplayName = "EnsureCanAssign should return failure when user changes own role")]
    public void EnsureCanAssign_Should_Return_Failure_When_User_Changes_Own_Role()
    {
        var actor = UserTestFactory.CreateUser();

        var result = UserRoleAssignmentPolicy.EnsureCanAssign(
            actor,
            actor,
            UserRole.Admin);

        var error = result.ShouldHaveSingleError(
            ErrorType.Validation,
            "User cannot change own role.");

        error.ShouldHaveField("Role");
    }

    [Fact(DisplayName = "EnsureCanAssign should return failure when non-admin assigns privileged role")]
    public void EnsureCanAssign_Should_Return_Failure_When_Non_Admin_Assigns_Privileged_Role()
    {
        var actor = UserTestFactory.CreateUser(role: "User", email: "actor@example.com");
        var target = UserTestFactory.CreateUser(role: "User", email: "target@example.com");

        var result = UserRoleAssignmentPolicy.EnsureCanAssign(
            actor,
            target,
            UserRole.Moderator);

        var error = result.ShouldHaveSingleError(
            ErrorType.Validation,
            "Only admins can assign privileged roles.");

        error.ShouldHaveField("Role");
    }

    [Fact(DisplayName = "EnsureCanAssign should return failure when non-admin assigns admin role")]
    public void EnsureCanAssign_Should_Return_Failure_When_Non_Admin_Assigns_Admin_Role()
    {
        var actor = UserTestFactory.CreateUser(role: "Moderator", email: "actor@example.com");
        var target = UserTestFactory.CreateUser(role: "User", email: "target@example.com");

        var result = UserRoleAssignmentPolicy.EnsureCanAssign(
            actor,
            target,
            UserRole.Admin);

        var error = result.ShouldHaveSingleError(
            ErrorType.Validation,
            "Only admins can assign privileged roles.");

        error.ShouldHaveField("Role");
    }

    [Fact(DisplayName = "EnsureCanAssign should return failure when non-admin changes admin role")]
    public void EnsureCanAssign_Should_Return_Failure_When_Non_Admin_Changes_Admin_Role()
    {
        var actor = UserTestFactory.CreateUser(role: "User", email: "actor@example.com");
        var target = UserTestFactory.CreateUser(role: "Admin", email: "target@example.com");

        var result = UserRoleAssignmentPolicy.EnsureCanAssign(
            actor,
            target,
            UserRole.User);

        var error = result.ShouldHaveSingleError(
            ErrorType.Validation,
            "Only admins can change another admin role.");

        error.ShouldHaveField("Role");
    }

    [Fact(DisplayName = "EnsureCanAssign should return success when admin assigns privileged role")]
    public void EnsureCanAssign_Should_Return_Success_When_Admin_Assigns_Privileged_Role()
    {
        var actor = UserTestFactory.CreateUser(role: "Admin", email: "actor@example.com");
        var target = UserTestFactory.CreateUser(role: "User", email: "target@example.com");

        var result = UserRoleAssignmentPolicy.EnsureCanAssign(
            actor,
            target,
            UserRole.Moderator);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(DisplayName = "EnsureCanAssign should return success when non-admin assigns regular role to non-admin")]
    public void EnsureCanAssign_Should_Return_Success_When_Non_Admin_Assigns_Regular_Role_To_Non_Admin()
    {
        var actor = UserTestFactory.CreateUser(role: "Moderator", email: "actor@example.com");
        var target = UserTestFactory.CreateUser(role: "User", email: "target@example.com");

        var result = UserRoleAssignmentPolicy.EnsureCanAssign(
            actor,
            target,
            UserRole.User);

        result.IsSuccess.ShouldBeTrue();
    }
}
