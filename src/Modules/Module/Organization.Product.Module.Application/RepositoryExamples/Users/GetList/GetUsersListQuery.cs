namespace Organization.Product.Module.Application.RepositoryExamples.Users.GetList;

public sealed record GetUsersListQuery(
    UsersFilterParameters FilterParameters,
    PaginationParameters PaginationParameters,
    SortingParameters SortingParameters)
    : IQuery<Result<PaginationResult<UserListItemReadModel>>>;
