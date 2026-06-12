using Organization.Product.Module.Domain.Users.ValueObjects;

namespace Organization.Product.Module.Domain.Users.Events;

public sealed record UserEmailChanged(
    UserId UserId,
    Email OldEmail,
    Email NewEmail) : DomainEvent;
