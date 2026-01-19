using Common.SearchParams;

using Dal.Ef.DbModels;
using Dal.Ef.Filters.Core;

namespace Dal.Ef.Filters;

public sealed class UsersFilter : IFilter<Guid, UserDbModel, UsersSearchParams>
{
    public static IQueryable<UserDbModel> Filter(IQueryable<UserDbModel> dbObjects, UsersSearchParams searchParams)
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