namespace Presentation.Features.Users.GetDetails;

public sealed record UserDetailsResponse(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar);
