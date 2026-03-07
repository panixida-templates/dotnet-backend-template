using Infrastructure.Persistence.Ef.Core;

namespace Infrastructure.Persistence.Ef.Features.Users;

internal sealed class UserDbModel : AuditableDbModel<Guid>
{
    public string Role { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string? Avatar { get; set; }
}
