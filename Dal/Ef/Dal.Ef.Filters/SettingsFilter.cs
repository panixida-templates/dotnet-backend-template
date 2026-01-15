using Common.SearchParams;

using Dal.Ef.DbModels;
using Dal.Ef.Filters.Core;

namespace Dal.Ef.Filters;

public sealed class SettingsFilter : IFilter<int, SettingDbModel, SettingsSearchParams>
{
    public static IQueryable<SettingDbModel> Filter(IQueryable<SettingDbModel> dbObjects, SettingsSearchParams searchParams)
    {
        return dbObjects;
    }
}
