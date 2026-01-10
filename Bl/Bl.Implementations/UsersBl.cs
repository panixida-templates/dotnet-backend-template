using Bl.Implementations.Core;
using Bl.Interfaces;

using Common.ConvertParams;
using Common.SearchParams;

using Dal.Interfaces;

using Entities;

namespace Bl.Implementations;

public sealed class UsersBl(IUsersDal usersDal) :
    BaseBl<int, User, UsersSearchParams, UsersConvertParams>(usersDal),
    IUsersBl
{
}

