namespace Application.Users.Get;

public sealed record GetByIdUserDto(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar);
