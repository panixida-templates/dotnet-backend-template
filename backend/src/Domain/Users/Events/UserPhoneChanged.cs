using Domain.Abstractions;

namespace Domain.Users.Events;

public sealed record UserPhoneChanged(
    Guid UserId,
    string OldPhone,
    string NewPhone) : DomainEvent;
