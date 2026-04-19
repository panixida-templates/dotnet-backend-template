using Domain.Users.Enumerations;
using Domain.Users.Events;
using Domain.Users.ValueObjects;

namespace Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    private const int MinimumAge = 18;

    private User(
        UserId id,
        UserRole role,
        UserName name,
        Email email,
        PhoneNumber phone,
        BirthDate birthDate,
        Avatar? avatar)
        : base(id)
    {
        Role = role;
        Name = name;
        Email = email;
        Phone = phone;
        BirthDate = birthDate;
        Avatar = avatar;
    }

    public UserRole Role { get; private set; }
    public UserName Name { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber Phone { get; private set; }
    public BirthDate BirthDate { get; private set; }
    public Avatar? Avatar { get; private set; }

    public static Result<User> Create(
        string role,
        string name,
        string email,
        string phone,
        DateOnly birthDate,
        string? avatar)
    {
        var roleResult = UserRole.Create(role);
        var nameResult = UserName.Create(name);
        var emailResult = Email.Create(email);
        var phoneResult = PhoneNumber.Create(phone);
        var birthDateResult = BirthDate.Create(birthDate);
        var avatarResult = Avatar.Create(avatar);

        var validationResult = Result.Combine(
            roleResult,
            nameResult,
            emailResult,
            phoneResult,
            birthDateResult,
            avatarResult);

        if (validationResult.IsFailure)
        {
            return Result.Failure<User>(validationResult.Errors);
        }

        var user = new User(
            UserId.New(),
            roleResult.Value,
            nameResult.Value,
            emailResult.Value,
            phoneResult.Value,
            birthDateResult.Value,
            avatarResult.Value);

        return Result.Success(user);
    }

    public int GetAge(DateOnly from)
    {
        return BirthDate.GetAge(from);
    }

    public void ChangeRole(UserRole role)
    {
        if (Role == role)
        {
            return;
        }
        Role = role;
    }

    public void ChangeName(UserName name)
    {
        if (Name == name)
        {
            return;
        }
        Name = name;
    }

    public void ChangeEmail(Email email)
    {
        if (Email == email)
        {
            return;
        }

        var oldEmail = Email;
        Email = email;

        AddDomainEvent(
            new UserEmailChanged(
                Id,
                oldEmail,
                Email));
    }

    public void ChangePhone(PhoneNumber phone)
    {
        if (Phone == phone)
        {
            return;
        }
        Phone = phone;
    }

    public Result ChangeBirthDate(BirthDate birthDate)
    {
        var birthDateValidationResult = ValidateBirthDate(birthDate);

        if (birthDateValidationResult.IsFailure)
        {
            return birthDateValidationResult;
        }

        if (BirthDate == birthDate)
        {
            return Result.Success();
        }

        BirthDate = birthDate;
        return Result.Success();
    }

    public void ChangeAvatar(Avatar? avatar)
    {
        if (Avatar == avatar)
        {
            return;
        }
        Avatar = avatar;
    }

    private static Result ValidateBirthDate(BirthDate birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return birthDate.EnsureAtLeast(MinimumAge, today);
    }
}
