using Common.ConvertParams;

using Dal.Ef.DbModels;

namespace Dal.Ef.Implementations.Includes;

internal static class SettingsIncludes
{
    internal static IQueryable<SettingDbModel> Include(this IQueryable<SettingDbModel> dbObjects, SettingsConvertParams convertParams)
    {
        return dbObjects;
    }
}
