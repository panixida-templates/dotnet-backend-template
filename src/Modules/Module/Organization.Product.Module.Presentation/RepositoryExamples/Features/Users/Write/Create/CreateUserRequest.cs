namespace Organization.Product.Module.Presentation.RepositoryExamples.Features.Users.Write.Create;

internal sealed record CreateUserRequest(
    string Role,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar);
