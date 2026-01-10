using Common.SearchParams;

using Dal.DbModels;

namespace Dal.Implementations.Filters;

internal static class SettingsFilters
{
    internal static IQueryable<SettingDbModel> Filter(this IQueryable<SettingDbModel> dbObjects, SettingsSearchParams searchParams)
    {
        return dbObjects;
    }
}
