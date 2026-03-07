using Application.Abstractions.Mediator;
using Application.Abstractions.Queries;
using Application.Users.Abstractions;

namespace Application.Users.Search;

public sealed class SearchUsersHandler(IUsersQueryService usersQueryService) : IQueryHandler<SearchUsersQuery, SearchResult<SearchUserDto>>
{
    public Task<SearchResult<SearchUserDto>> HandleAsync(SearchUsersQuery query, CancellationToken cancellationToken)
    {
        return usersQueryService.SearchAsync(query.SearchParams, cancellationToken);
    }
}
