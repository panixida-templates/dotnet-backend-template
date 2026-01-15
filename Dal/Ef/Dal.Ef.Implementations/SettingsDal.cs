using Common.ConvertParams;
using Common.SearchParams;

using Dal.Ef.DbModels;
using Dal.Ef.Filters;
using Dal.Ef.Implementations.Core;
using Dal.Ef.Includes;
using Dal.Ef.Mappers;

using Dal.Interfaces;

using Entities;

namespace Dal.Ef.Implementations;

public sealed class SettingsDal(DefaultDbContext context) : 
    BaseDal<DefaultDbContext, int, SettingDbModel, Setting, SettingsSearchParams, SettingsConvertParams, SettingsMapper, SettingsFilter, SettingsInclude>(context),
    ISettingsDal
{
}
