using Application.Abstractions.Mediator;
using Application.Abstractions.Persistence;

using Common.SearchParams.Core;

using Domain.Entities;

namespace Application.Users.Search;

public sealed class SearchUsersHandler(IUsersRepository usersRepository) : IQueryHandler<SearchUsersQuery, SearchResult<User>>
{
    public Task<SearchResult<User>> HandleAsync(SearchUsersQuery query, CancellationToken cancellationToken)
    {
        return usersRepository.GetAsync(query.SearchParams, query.ConvertParams);
    }
}
