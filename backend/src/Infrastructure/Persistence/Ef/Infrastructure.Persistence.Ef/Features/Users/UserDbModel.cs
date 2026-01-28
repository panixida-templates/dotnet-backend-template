using Domain.Enums;

using Infrastructure.Persistence.Ef.Core;

namespace Infrastructure.Persistence.Ef.Features.Users;

internal sealed class UserDbModel : DbModel<Guid>
{
    public UserRole Role { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateOnly Birthday { get; set; }
    public string? Avatar { get; set; }
}
