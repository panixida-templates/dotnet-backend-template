using Common.ConvertParams;

using Infrastructure.Persistence.Ef.Core;

namespace Infrastructure.Persistence.Ef.Features.Users;

internal sealed class UsersInclude : IInclude<Guid, UserDbModel, UsersConvertParams>
{
    public static IQueryable<UserDbModel> Include(IQueryable<UserDbModel> dbObjects, UsersConvertParams convertParams)
    {
        return dbObjects;
    }
}
