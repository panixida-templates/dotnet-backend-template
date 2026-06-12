using PANiXiDA.Core.Application.Querying.Filtering;

namespace Organization.Product.Module.Application.Users.GetList;

public sealed record UsersFilterParameters(
    string? Role) : FilterParameters;
