using Domain.Abstractions;

namespace Domain.Users.Events;

public sealed record UserAvatarChanged(
    Guid UserId,
    string? OldAvatar,
    string? NewAvatar) : DomainEvent;
