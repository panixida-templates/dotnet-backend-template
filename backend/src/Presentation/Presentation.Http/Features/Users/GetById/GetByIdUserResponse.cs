namespace Presentation.Http.Features.Users.GetById;

public sealed record GetByIdUserResponse(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar);
