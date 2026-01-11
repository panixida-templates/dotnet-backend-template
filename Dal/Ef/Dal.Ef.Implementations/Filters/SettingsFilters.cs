using Common.SearchParams;

using Dal.Ef.DbModels;

namespace Dal.Ef.Implementations.Filters;

internal static class SettingsFilters
{
    internal static IQueryable<SettingDbModel> Filter(this IQueryable<SettingDbModel> dbObjects, SettingsSearchParams searchParams)
    {
        return dbObjects;
    }
}
