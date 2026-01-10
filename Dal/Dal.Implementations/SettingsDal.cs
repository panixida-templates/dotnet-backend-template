using Common.ConvertParams;
using Common.SearchParams;

using Dal.DbModels;
using Dal.Ef;
using Dal.Implementations.Core;
using Dal.Implementations.Filters;
using Dal.Implementations.Includes;
using Dal.Interfaces;
using Dal.Mappers;

using Entities;

using Microsoft.EntityFrameworkCore;

namespace Dal.Implementations;

public sealed class SettingsDal(DefaultDbContext context) : 
    BaseDal<DefaultDbContext, int, SettingDbModel, Setting, SettingsMapper, SettingsSearchParams, SettingsConvertParams>(context),
    ISettingsDal
{
    protected override async Task<IQueryable<SettingDbModel>> BuildDbQueryAsync(IQueryable<SettingDbModel> dbObjects, SettingsSearchParams searchParams)
    {
        dbObjects = await base.BuildDbQueryAsync(dbObjects, searchParams);
        return dbObjects.Filter(searchParams);
    }

    protected override async Task<IList<Setting>> BuildEntitiesListAsync(IQueryable<SettingDbModel> dbObjects, SettingsConvertParams convertParams)
    {
        return [.. (await dbObjects.Include(convertParams).ToListAsync()).Select(SettingsMapper.ToEntity)];
    }
}
