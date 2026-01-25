using Domain.Events.Core;

namespace Domain.Events.Users;

public sealed record UserAvatarChanged(Guid UserId, string? Avatar, DateTimeOffset OccurredOn) : DomainEvent(OccurredOn);
