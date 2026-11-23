namespace Organization.Product.Module.Application.RepositoryExamples.Users.Common.Filters;

public sealed record UsersFilterParameters(
    string? Role) : IFilter;
