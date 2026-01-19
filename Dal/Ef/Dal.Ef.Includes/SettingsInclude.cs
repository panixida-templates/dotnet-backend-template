using Common.ConvertParams;

using Dal.Ef.DbModels;
using Dal.Ef.Includes.Core;

namespace Dal.Ef.Includes;

public sealed class SettingsInclude : IInclude<int, SettingDbModel, SettingsConvertParams>
{
    public static IQueryable<SettingDbModel> Include(IQueryable<SettingDbModel> dbObjects, SettingsConvertParams convertParams)
    {
        return dbObjects;
    }
}
