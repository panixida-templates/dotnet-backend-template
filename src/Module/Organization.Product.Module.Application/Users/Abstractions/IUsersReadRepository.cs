using Organization.Product.Module.Application.Users.GetDetails;
using Organization.Product.Module.Application.Users.GetList;
using PANiXiDA.Core.Application.Persistence;
using PANiXiDA.Core.Application.Querying.Pagination;
using PANiXiDA.Core.Application.Querying.Sorting;

namespace Organization.Product.Module.Application.Users.Abstractions;

public interface IUsersReadRepository : IReadRepository<Guid>
{
    Task<UserDetailsReadModel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<PaginationResult<UserListItemReadModel>> GetPagedListAsync(
        UsersFilterParameters filterParameters,
        PaginationParameters paginationParameters,
        SortParameters sortParameters,
        CancellationToken cancellationToken);
}
