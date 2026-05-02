using Application.Abstractions.Persistence;
using Application.Users.Abstractions;

using Domain.Users;

using Infrastructure.Ef.Core.Write;
using Infrastructure.Ef.EfCore;

namespace Infrastructure.Ef.Features.Users.Write;

internal sealed class UsersRepository(TemplateWriteDbContext dbContext, IAggregateTracker aggregateTracker)
    : Repository<TemplateWriteDbContext, UserId, User>(dbContext, aggregateTracker),
    IUsersRepository
{
}
