using Application.Abstractions.Mediator;
using Application.Abstractions.Queries;

using Domain.Abstractions.ResultPattern;

namespace Application.Users.Search;

public sealed record SearchUsersQuery(UsersSearchParams SearchParams) : IQuery<Result<SearchResult<SearchUserDto>>>;
