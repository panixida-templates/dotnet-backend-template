using Domain.Entities.Core;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class User(
    Guid id,
    UserRole role,
    string name,
    Email email,
    PhoneNumber phone,
    DateOnly birthday,
    string? avatar) : Entity<Guid>(id)
{
    public UserRole Role { get; private set; } = role;
    public string Name { get; private set; } = name;
    public Email Email { get; private set; } = email;
    public PhoneNumber Phone { get; private set; } = phone;
    public DateOnly Birthday { get; private set; } = birthday;
    public string? Avatar { get; private set; } = avatar;

    public int GetAge(DateOnly from)
    {
        var years = from.Year - Birthday.Year;

        var hadBirthdayThisYear = from.Month > Birthday.Month || (from.Month == Birthday.Month && from.Day >= Birthday.Day);
        if (!hadBirthdayThisYear)
        {
            years--;
        }

        return years;
    }

    public void ChangeAvatar(string? avatar, DateTimeOffset occurredOn)
    {
        if (Avatar == avatar)
        {
            return;
        }

        Avatar = avatar;

        //AddDomainEvent(
        //    new UserAvatarChanged(Id, Avatar, occurredOn)
        //);
    }

}
