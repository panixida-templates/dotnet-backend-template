using Microsoft.EntityFrameworkCore;

using Organization.Product.Module.Application.RepositoryExamples.Users.Abstractions;
using Organization.Product.Module.Application.RepositoryExamples.Users.Common.Filters;
using Organization.Product.Module.Application.RepositoryExamples.Users.Read.GetDetails;
using Organization.Product.Module.Application.RepositoryExamples.Users.Read.GetList;
using Organization.Product.Module.Infrastructure.Persistence.Core;
using Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Read.DbModels;
using Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Read.GetDetails;
using Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Read.GetList;

namespace Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Read;

public sealed class UsersReadRepository(ModuleReadDbContext dbContext) :
    EfReadRepository<ModuleReadDbContext, Guid, UserReadDbModel>(dbContext),
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
        SortingParameters sortingParameters,
        CancellationToken cancellationToken)
    {
        var query = ApplyFilter(
            query: Query,
            filterParameters: filterParameters);

        return GetPagedResultAsync<UserListItemReadModel, UserListItemReadModelMapper, UserListItemReadModelSorting>(
            query,
            paginationParameters,
            sortingParameters,
            cancellationToken);
    }

    private static IQueryable<UserReadDbModel> ApplyFilter(
        IQueryable<UserReadDbModel> query,
        UsersFilterParameters filterParameters)
    {
        if (!string.IsNullOrWhiteSpace(filterParameters.Role))
        {
            query = query.Where(user =>
                EF.Functions.ILike(
                    matchExpression: user.Role,
                    pattern: filterParameters.Role));
        }

        return query;
    }
}
