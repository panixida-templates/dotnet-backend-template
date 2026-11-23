namespace Organization.Product.Module.Application.RepositoryExamples.Users.Read.GetDetails;

public sealed record UserDetailsReadModel(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string? Avatar) : IReadModel;
