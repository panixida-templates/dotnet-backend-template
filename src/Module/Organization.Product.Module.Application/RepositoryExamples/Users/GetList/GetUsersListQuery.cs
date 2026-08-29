namespace Organization.Product.Module.Application.RepositoryExamples.Users.GetList;

public sealed record GetUsersListQuery(
    UsersFilterParameters FilterParameters,
    PaginationParameters PaginationParameters,
    SortParameters SortParameters)
    : IQuery<Result<PaginationResult<UserListItemReadModel>>>;
