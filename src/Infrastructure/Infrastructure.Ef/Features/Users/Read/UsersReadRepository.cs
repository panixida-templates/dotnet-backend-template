using Application.Abstractions.Persistence.Read.Paged;
using Application.Abstractions.Persistence.Read.Sorting;
using Application.Users.Abstractions;
using Application.Users.GetDetails;
using Application.Users.GetList;

using Infrastructure.Ef.Core.Read;
using Infrastructure.Ef.EfCore;
using Infrastructure.Ef.Features.Users.Read.Filters;
using Infrastructure.Ef.Features.Users.Read.Mappers;

namespace Infrastructure.Ef.Features.Users.Read;

internal sealed class UsersReadRepository(DefaultDbContext dbContext) :
    ReadRepository<DefaultDbContext, Guid, UserReadDbModel>(dbContext),
    IUsersReadRepository
{
    public Task<UserDetailsReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return GetByIdAsync<UserDetailsReadModel, UserDetailsReadModelMapper>(
           id,
           cancellationToken);
    }

    public Task<PagedResult<UserListItemReadModel>> GetPagedListAsync(
        UsersFilterParameters filterParameters,
        PaginationParameters paginationParameters,
        SortParameters sortParameters,
        CancellationToken cancellationToken)
    {
        var query = Query;
        query = UsersFilter.Apply(query, filterParameters);

        return GetPagedResultAsync<UserListItemReadModel, UserListItemReadModelMapper>(
            query,
            paginationParameters,
            sortParameters,
            cancellationToken);
    }
}
