using Application.Abstractions.Mediator;

using Common.ConvertParams;
using Common.SearchParams;
using Common.SearchParams.Core;

using Domain.Entities;

namespace Application.Users.Search;

public sealed record SearchUsersQuery(UsersSearchParams SearchParams, UsersConvertParams? ConvertParams) : IQuery<SearchResult<User>>;
