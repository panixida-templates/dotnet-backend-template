using Common.SearchParams;

using Dal.Ef.DbModels;

namespace Dal.Ef.Implementations.Filters;

internal static class UsersFilters
{
    internal static IQueryable<UserDbModel> Filter(this IQueryable<UserDbModel> dbObjects, UsersSearchParams searchParams)
    {
        if (searchParams.Role.HasValue)
        {
            dbObjects = dbObjects.Where(item => item.Role == searchParams.Role.Value);
        }
        if (!string.IsNullOrEmpty(searchParams.SearchQuery))
        {
            dbObjects = dbObjects.Where(item => item.Name.Contains(searchParams.SearchQuery) || item.Email.Contains(searchParams.SearchQuery));
        }

        return dbObjects;
    }
}
