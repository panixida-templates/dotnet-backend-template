namespace Application.Users.Search;

public sealed record SearchUserDto(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar);
