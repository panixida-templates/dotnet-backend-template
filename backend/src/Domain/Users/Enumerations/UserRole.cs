using Domain.Abstractions;

namespace Domain.Users.Enumerations;

public sealed class UserRole : Enumeration<UserRole>
{
    public static readonly UserRole User = new(1, nameof(User));
    public static readonly UserRole Admin = new(2, nameof(Admin));
    public static readonly UserRole Moderator = new(3, nameof(Moderator));

    private UserRole(int id, string name)
        : base(id, name)
    {
    }
}
