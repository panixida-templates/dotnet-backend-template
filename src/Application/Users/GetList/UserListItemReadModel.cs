namespace Application.Users.GetList;

public sealed record UserListItemReadModel(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar);
