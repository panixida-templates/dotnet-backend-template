using Common.Enums;

using Dal.Ef.DbModels.Core;

namespace Dal.Ef.DbModels;

public sealed class UserDbModel : BaseDbModel<Guid>
{
    public Role Role { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public int Age { get; set; }
    public DateTime Birthday { get; set; }
}
