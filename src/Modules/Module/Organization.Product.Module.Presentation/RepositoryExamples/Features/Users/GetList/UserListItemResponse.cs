namespace Organization.Product.Module.Presentation.RepositoryExamples.Features.Users.GetList;

internal sealed record UserListItemResponse(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar);
