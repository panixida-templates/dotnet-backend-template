using Application.Abstractions.Mediator;
using Application.Abstractions.Queries;

namespace Application.Users.Search;

public sealed record SearchUsersQuery(UsersSearchParams SearchParams) : IQuery<SearchResult<SearchUserDto>>;
