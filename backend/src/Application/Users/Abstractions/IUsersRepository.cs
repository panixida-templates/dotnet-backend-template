using Application.Abstractions.Persistence;

using Domain.Users;

namespace Application.Users.Abstractions;

public interface IUsersRepository : IRepository<Guid, User>
{
}
