using Application.Users.Abstractions;
using Application.Users.Get;
using Application.Users.Search;

using Infrastructure.Persistence.Ef.Core;
using Infrastructure.Persistence.Ef.EfCore;

namespace Infrastructure.Persistence.Ef.Features.Users;

internal sealed class UsersQueryService(DefaultDbContext dbContext) :
    QueryService<DefaultDbContext, Guid, UserDbModel, GetByIdUserDto, SearchUserDto, UsersSearchParams, GetByIdUserDtoMapper, SearchUserDtoMapper, UsersFilter>(dbContext),
    IUsersQueryService
{
}
