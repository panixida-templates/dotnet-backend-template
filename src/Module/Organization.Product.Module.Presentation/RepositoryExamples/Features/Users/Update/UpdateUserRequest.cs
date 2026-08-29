namespace Organization.Product.Module.Presentation.RepositoryExamples.Features.Users.Update;

internal sealed record UpdateUserRequest(
    string Role,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar);
