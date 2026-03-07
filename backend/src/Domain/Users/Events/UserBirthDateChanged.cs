using Domain.Abstractions;

namespace Domain.Users.Events;

public sealed record UserBirthDateChanged(
    Guid UserId,
    DateOnly OldBirthDate,
    DateOnly NewBirthDate) : DomainEvent;
