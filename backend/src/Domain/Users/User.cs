using Domain.Abstractions;
using Domain.Users.Enumerations;
using Domain.Users.Events;
using Domain.Users.ValueObjects;

namespace Domain.Users;

public sealed class User : AggregateRoot<Guid>
{
    private const int MinimumAge = 18;

    private User(
        Guid id,
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

    public static User Create(
        Guid id,
        UserRole role,
        UserName name,
        Email email,
        PhoneNumber phone,
        BirthDate birthDate,
        Avatar? avatar)
    {
        EnsureBirthDateIsValid(birthDate);

        var user = new User(
            id,
            role,
            name,
            email,
            phone,
            birthDate,
            avatar);

        user.AddDomainEvent(
            new UserCreated(
                user.Id,
                user.Role.Id,
                user.Role.Name,
                user.Name.Value,
                user.Email.Value,
                user.Phone.Value,
                user.BirthDate.Value,
                user.Avatar?.Value));

        return user;
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

        var oldRole = Role;
        Role = role;

        AddDomainEvent(
            new UserRoleChanged(
                Id,
                oldRole.Id,
                oldRole.Name,
                Role.Id,
                Role.Name));
    }

    public void ChangeName(UserName name)
    {
        if (Name == name)
        {
            return;
        }

        var oldName = Name;
        Name = name;

        AddDomainEvent(
            new UserNameChanged(
                Id,
                oldName.Value,
                Name.Value));
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
                oldEmail.Value,
                Email.Value));
    }

    public void ChangePhone(PhoneNumber phone)
    {
        if (Phone == phone)
        {
            return;
        }

        var oldPhone = Phone;
        Phone = phone;

        AddDomainEvent(
            new UserPhoneChanged(
                Id,
                oldPhone.Value,
                Phone.Value));
    }

    public void ChangeBirthDate(BirthDate birthDate)
    {
        EnsureBirthDateIsValid(birthDate);

        if (BirthDate == birthDate)
        {
            return;
        }

        var oldBirthDate = BirthDate;
        BirthDate = birthDate;

        AddDomainEvent(
            new UserBirthDateChanged(
                Id,
                oldBirthDate.Value,
                BirthDate.Value));
    }

    public void ChangeAvatar(Avatar? avatar)
    {
        if (Avatar == avatar)
        {
            return;
        }

        var oldAvatar = Avatar;
        Avatar = avatar;

        AddDomainEvent(
            new UserAvatarChanged(
                Id,
                oldAvatar?.Value,
                Avatar?.Value));
    }

    private static void EnsureBirthDateIsValid(BirthDate birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        birthDate.EnsureAtLeast(MinimumAge, today);
    }
}