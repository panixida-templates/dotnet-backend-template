using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence.Read.Paged;
using Application.Users.Abstractions;

namespace Application.Users.GetList;

public sealed class GetUsersListHandler(IUsersReadRepository usersQueryService)
    : IQueryHandler<GetUsersListQuery, Result<PagedResult<UserListItemReadModel>>>
{
    public async Task<Result<PagedResult<UserListItemReadModel>>> HandleAsync(
        GetUsersListQuery query,
        CancellationToken cancellationToken)
    {
        var users = await usersQueryService.GetPagedListAsync(
            query.FilterParameters,
            query.PaginationParameters,
            query.SortParameters,
            cancellationToken);

        return Result.Success(users);
    }
}
