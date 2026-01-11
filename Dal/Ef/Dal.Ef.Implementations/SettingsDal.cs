using Common.ConvertParams;
using Common.SearchParams;

using Dal.Ef.DbModels;
using Dal.Ef.Implementations.Core;
using Dal.Ef.Implementations.Filters;
using Dal.Ef.Implementations.Includes;
using Dal.Ef.Mappers;

using Dal.Interfaces;

using Entities;

using Microsoft.EntityFrameworkCore;

namespace Dal.Ef.Implementations;

public sealed class SettingsDal(DefaultDbContext context) : 
    BaseDal<DefaultDbContext, int, SettingDbModel, Setting, SettingsMapper, SettingsSearchParams, SettingsConvertParams>(context),
    ISettingsDal
{
    protected override async ValueTask<IQueryable<SettingDbModel>> BuildDbFilterAsync(IQueryable<SettingDbModel> dbObjects, SettingsSearchParams searchParams)
    {
        dbObjects = await base.BuildDbFilterAsync(dbObjects, searchParams);
        return dbObjects.Filter(searchParams);
    }

    protected override async Task<IList<Setting>> BuildEntitiesListAsync(IQueryable<SettingDbModel> dbObjects, SettingsConvertParams convertParams)
    {
        return [.. (await dbObjects.Include(convertParams).ToListAsync()).Select(SettingsMapper.ToEntity)];
    }
}
