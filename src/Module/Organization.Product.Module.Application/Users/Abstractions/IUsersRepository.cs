using Organization.Product.Module.Domain.Users;

using PANiXiDA.Core.Application.Persistence;

namespace Organization.Product.Module.Application.Users.Abstractions;

public interface IUsersRepository : IRepository<UserId, User>
{
}
