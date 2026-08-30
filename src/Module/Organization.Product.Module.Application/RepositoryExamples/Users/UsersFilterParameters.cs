namespace Organization.Product.Module.Application.RepositoryExamples.Users;

public sealed record UsersFilterParameters(
    string? Role) : FilterParameters;
