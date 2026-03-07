using Application.Users.Abstractions;

using Domain.Users;

using Infrastructure.Persistence.Ef.Core;
using Infrastructure.Persistence.Ef.EfCore;

namespace Infrastructure.Persistence.Ef.Features.Users;

internal sealed class UsersRepository(DefaultDbContext dbContext) :
    Repository<DefaultDbContext, Guid, UserDbModel, User, UserMapper>(dbContext),
    IUsersRepository
{
}
