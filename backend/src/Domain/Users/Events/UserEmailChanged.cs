using Domain.Abstractions;
using Domain.Users.ValueObjects;

namespace Domain.Users.Events;

public sealed record UserEmailChanged(
    UserId UserId,
    Email OldEmail,
    Email NewEmail) : DomainEvent;
