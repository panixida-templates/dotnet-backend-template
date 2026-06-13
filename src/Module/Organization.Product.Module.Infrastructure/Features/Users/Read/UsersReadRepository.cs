using Infrastructure.Ef.Features.Users.Read.Mappers;
using Organization.Product.Module.Application.Users;
using Organization.Product.Module.Application.Users.Abstractions;
using Organization.Product.Module.Application.Users.GetDetails;
using Organization.Product.Module.Application.Users.GetList;
using Organization.Product.Module.Infrastructure.EfCore;
using Organization.Product.Module.Infrastructure.Features.Users.Read.Filters;
using PANiXiDA.Core.Application.Querying.Pagination;
using PANiXiDA.Core.Application.Querying.Sorting;
using PANiXiDA.Core.Infrastructure.Persistence.Ef.Read;

namespace Organization.Product.Module.Infrastructure.Features.Users.Read;

internal sealed class UsersReadRepository(TemplateReadDbContext dbContext) :
    EfReadRepository<TemplateReadDbContext, Guid, UserReadDbModel>(dbContext),
    IUsersReadRepository
{
    public Task<UserDetailsReadModel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return GetByIdAsync<UserDetailsReadModel, UserDetailsReadModelMapper>(
           id,
           cancellationToken);
    }

    public Task<PaginationResult<UserListItemReadModel>> GetPagedListAsync(
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
