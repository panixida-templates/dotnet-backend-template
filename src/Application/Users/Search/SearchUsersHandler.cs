using Application.Abstractions.Mediator;
using Application.Abstractions.Queries;
using Application.Users.Abstractions;

namespace Application.Users.Search;

public sealed class SearchUsersHandler(IUsersQueryService usersQueryService) : IQueryHandler<SearchUsersQuery, Result<SearchResult<SearchUserDto>>>
{
    public async Task<Result<SearchResult<SearchUserDto>>> HandleAsync(SearchUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await usersQueryService.SearchAsync(query.SearchParams, cancellationToken);
        return Result.Success(users);
    }
}
