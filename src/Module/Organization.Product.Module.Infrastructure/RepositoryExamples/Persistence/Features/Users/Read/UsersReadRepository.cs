using Organization.Product.Module.Application.RepositoryExamples.Users;
using Organization.Product.Module.Application.RepositoryExamples.Users.Abstractions;
using Organization.Product.Module.Application.RepositoryExamples.Users.GetDetails;
using Organization.Product.Module.Application.RepositoryExamples.Users.GetList;
using Organization.Product.Module.Infrastructure.Persistence.Core;
using Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Read.Filters;
using Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Read.Mappers;

namespace Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Read;

public sealed class UsersReadRepository(TemplateReadDbContext dbContext) :
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
