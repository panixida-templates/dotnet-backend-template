using Domain.Abstractions;

namespace Domain.Users.Events;

public sealed record UserCreated(
    Guid UserId,
    int RoleId,
    string RoleName,
    string Name,
    string Email,
    string Phone,
    DateOnly Birthday,
    string? Avatar) : DomainEvent;
