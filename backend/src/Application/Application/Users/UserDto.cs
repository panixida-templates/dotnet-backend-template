using Domain.Enums;

namespace Application.Users;

public sealed record UserDto(
    Guid Id,
    UserRole Role,
    string Name,
    string Email,
    string Phone,
    DateOnly Birthday,
    string? AvatarKey);
