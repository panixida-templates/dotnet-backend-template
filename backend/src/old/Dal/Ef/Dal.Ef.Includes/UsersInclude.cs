using Common.ConvertParams;

using Dal.Ef.DbModels;
using Dal.Ef.Includes.Core;

namespace Dal.Ef.Includes;

public sealed class UsersInclude : IInclude<Guid, UserDbModel, UsersConvertParams>
{
    public static IQueryable<UserDbModel> Include(IQueryable<UserDbModel> dbObjects, UsersConvertParams convertParams)
    {
        return dbObjects;
    }
}
