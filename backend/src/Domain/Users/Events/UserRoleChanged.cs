using Domain.Abstractions;

namespace Domain.Users.Events;

public sealed record UserRoleChanged(
    Guid UserId,
    int OldRoleId,
    string OldRoleName,
    int NewRoleId,
    string NewRoleName) : DomainEvent;
