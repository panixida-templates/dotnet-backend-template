using Organization.Product.Module.Application.Users.Abstractions;
using Organization.Product.Module.Domain.Users;
using Organization.Product.Module.Infrastructure.EfCore;

using PANiXiDA.Core.Application.Persistence;
using PANiXiDA.Core.Infrastructure.Persistence.Ef.Write;

namespace Organization.Product.Module.Infrastructure.Features.Users.Write;

internal sealed class UsersRepository(TemplateWriteDbContext dbContext, IAggregateTracker aggregateTracker)
    : EfWriteRepository<TemplateWriteDbContext, UserId, User>(dbContext, aggregateTracker),
    IUsersRepository
{
}
