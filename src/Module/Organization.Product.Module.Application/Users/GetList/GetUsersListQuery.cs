using PANiXiDA.Core.Application.Messaging.Mediator.Contracts;
using PANiXiDA.Core.Application.Querying.Pagination;
using PANiXiDA.Core.Application.Querying.Sorting;

namespace Organization.Product.Module.Application.Users.GetList;

public sealed record GetUsersListQuery(
    UsersFilterParameters FilterParameters,
    PaginationParameters PaginationParameters,
    SortParameters SortParameters)
    : IQuery<Result<PaginationResult<UserListItemReadModel>>>;
