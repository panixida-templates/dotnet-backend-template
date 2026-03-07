using Domain.Abstractions;

namespace Domain.Users.Events;

public sealed record UserNameChanged(
    Guid UserId,
    string OldName,
    string NewName) : DomainEvent;
