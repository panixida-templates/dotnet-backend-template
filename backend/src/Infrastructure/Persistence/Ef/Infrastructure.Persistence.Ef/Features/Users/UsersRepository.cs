using Application.Abstractions.Persistence;

using Common.ConvertParams;
using Common.SearchParams;

using Domain.Entities;

using Infrastructure.Persistence.Ef.Core;
using Infrastructure.Persistence.Ef.EfCore;

namespace Infrastructure.Persistence.Ef.Features.Users;

internal sealed class UsersRepository(DefaultDbContext context) :
    EfRepository<DefaultDbContext, Guid, UserDbModel, User, UsersSearchParams, UsersConvertParams, UsersMapper, UsersFilter, UsersInclude>(context),
    IUsersRepository
{
}
