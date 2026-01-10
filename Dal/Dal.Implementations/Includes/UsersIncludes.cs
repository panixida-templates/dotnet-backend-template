using Common.ConvertParams;

using Dal.DbModels;

namespace Dal.Implementations.Includes;

internal static class UsersIncludes
{
    internal static IQueryable<UserDbModel> Include(this IQueryable<UserDbModel> dbObjects, UsersConvertParams convertParams)
    {
        return dbObjects;
    }
}
