using Common.Enums;

using Entities.Core;

namespace Entities;

public sealed class User : BaseEntity<Guid>
{
    public Role Role { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int Age { get; set; }
    public DateTime Birthday { get; set; }
    public string? Avatar { get; set; }

    public User(
        Guid id,
        Role role,
        string name,
        string email,
        string phone,
        int age,
        DateTime birthday,
        string? avatar) : base(id)
    {
        Role = role;
        Name = name;
        Email = email;
        Phone = phone;
        Age = age;
        Birthday = birthday;
        Avatar = avatar;
    }
}
