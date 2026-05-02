using Application.Abstractions.Persistence.Read.Filtering;

namespace Application.Users.GetList;

public sealed record UsersFilterParameters(
    string? Role) : FilterParameters;
