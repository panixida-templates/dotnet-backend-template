using Common.ConvertParams;

using Dal.DbModels;

namespace Dal.Implementations.Includes;

internal static class SettingsIncludes
{
    internal static IQueryable<SettingDbModel> Include(this IQueryable<SettingDbModel> dbObjects, SettingsConvertParams convertParams)
    {
        return dbObjects;
    }
}
