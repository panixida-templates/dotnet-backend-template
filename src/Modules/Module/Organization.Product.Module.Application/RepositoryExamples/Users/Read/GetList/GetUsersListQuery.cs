using Organization.Product.Module.Application.RepositoryExamples.Users.Common.Filters;

namespace Organization.Product.Module.Application.RepositoryExamples.Users.Read.GetList;

public sealed record GetUsersListQuery(
    UsersFilterParameters FilterParameters,
    PaginationParameters PaginationParameters,
    SortingParameters SortingParameters)
    : IQuery<Result<PaginationResult<UserListItemReadModel>>>;
