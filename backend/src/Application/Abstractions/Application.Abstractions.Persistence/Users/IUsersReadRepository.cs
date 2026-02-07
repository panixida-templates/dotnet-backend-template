using Application.Abstractions.Persistence.Core;

using Common.SearchParams;

using Domain.Entities;

namespace Application.Abstractions.Persistence.Users;

public interface IUsersReadRepository : IReadRepository<Guid, User, UsersSearchParams>
{
}
