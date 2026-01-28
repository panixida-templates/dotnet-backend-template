using Application.Abstractions.Persistence.Core;

using Common.ConvertParams;
using Common.SearchParams;

using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IUsersRepository : IRepository<Guid, User, UsersSearchParams, UsersConvertParams>
{
}
