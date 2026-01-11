using Common.ConvertParams;

using Dal.Ef.DbModels;

namespace Dal.Ef.Implementations.Includes;

internal static class UsersIncludes
{
    internal static IQueryable<UserDbModel> Include(this IQueryable<UserDbModel> dbObjects, UsersConvertParams convertParams)
    {
        return dbObjects;
    }
}
