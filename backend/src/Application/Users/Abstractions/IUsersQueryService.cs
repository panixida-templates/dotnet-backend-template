using Application.Abstractions.Queries;
using Application.Users.Get;
using Application.Users.Search;

namespace Application.Users.Abstractions;

public interface IUsersQueryService : IQueryService<Guid, GetByIdUserDto, SearchUserDto, UsersSearchParams>
{
}
