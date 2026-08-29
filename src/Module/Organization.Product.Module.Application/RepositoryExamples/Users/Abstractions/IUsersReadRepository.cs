using Organization.Product.Module.Application.RepositoryExamples.Users.GetDetails;
using Organization.Product.Module.Application.RepositoryExamples.Users.GetList;

namespace Organization.Product.Module.Application.RepositoryExamples.Users.Abstractions;

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
