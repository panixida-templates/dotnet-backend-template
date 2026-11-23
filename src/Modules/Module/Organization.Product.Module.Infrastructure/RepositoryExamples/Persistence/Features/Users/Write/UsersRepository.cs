using Organization.Product.Module.Domain.RepositoryExamples.Users;
using Organization.Product.Module.Domain.RepositoryExamples.Users.Abstractions;
using Organization.Product.Module.Infrastructure.Persistence.Core;

namespace Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Write;

public sealed class UsersRepository(
    ModuleWriteDbContext dbContext)
    : EfRepository<ModuleWriteDbContext, UserId, User>(dbContext),
    IUsersRepository;
