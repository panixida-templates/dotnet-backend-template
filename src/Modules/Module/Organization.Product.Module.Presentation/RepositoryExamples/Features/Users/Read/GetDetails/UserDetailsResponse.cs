namespace Organization.Product.Module.Presentation.RepositoryExamples.Features.Users.Read.GetDetails;

internal sealed record UserDetailsResponse(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar);
