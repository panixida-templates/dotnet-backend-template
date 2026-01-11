using Common.ConvertParams;
using Common.SearchParams;

using Dal.Interfaces.Core;

using Entities;

namespace Dal.Interfaces;

public interface IUsersDal : IBaseDal<Guid, User, UsersSearchParams, UsersConvertParams>
{
}