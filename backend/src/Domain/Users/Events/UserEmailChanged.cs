using Domain.Abstractions;

namespace Domain.Users.Events;

public sealed record UserEmailChanged(
    Guid UserId,
    string OldEmail,
    string NewEmail) : DomainEvent;
