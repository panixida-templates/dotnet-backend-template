using Application.Abstractions.Persistence;
using Application.Users.Abstractions;

using Domain.Users;

using Infrastructure.Persistence.Ef.Core;
using Infrastructure.Persistence.Ef.EfCore;

namespace Infrastructure.Persistence.Ef.Features.Users;

internal sealed class UsersRepository(DefaultDbContext dbContext, IAggregateTracker aggregateTracker)
    : Repository<DefaultDbContext, Guid, UserDbModel, User, UserMapper>(dbContext, aggregateTracker),
    IUsersRepository
{
}
