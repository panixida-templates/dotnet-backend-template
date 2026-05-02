using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence.Read.Paged;
using Application.Abstractions.Persistence.Read.Sorting;

namespace Application.Users.GetList;

public sealed record GetUsersListQuery(
    UsersFilterParameters FilterParameters,
    PaginationParameters PaginationParameters,
    SortParameters SortParameters)
    : IQuery<Result<PagedResult<UserListItemReadModel>>>;
