using Application.Abstractions.Persistence.Read;
using Application.Abstractions.Persistence.Read.Paged;
using Application.Abstractions.Persistence.Read.Sorting;
using Application.Users.GetDetails;
using Application.Users.GetList;

namespace Application.Users.Abstractions;

public interface IUsersReadRepository : IReadRepository<Guid>
{
    Task<UserDetailsReadModel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<PagedResult<UserListItemReadModel>> GetPagedListAsync(
        UsersFilterParameters filterParameters,
        PaginationParameters paginationParameters,
        SortParameters sortParameters,
        CancellationToken cancellationToken);
}
