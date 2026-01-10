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

public sealed class UsersDal(DefaultDbContext context) : 
    BaseDal<DefaultDbContext, int, UserDbModel, User, UsersMapper, UsersSearchParams, UsersConvertParams>(context),
    IUsersDal
{
    protected override async Task<IQueryable<UserDbModel>> BuildDbQueryAsync(IQueryable<UserDbModel> dbObjects, UsersSearchParams searchParams)
    {
        dbObjects = await base.BuildDbQueryAsync(dbObjects, searchParams);
        return dbObjects.Filter(searchParams);
    }

    protected override async Task<IList<User>> BuildEntitiesListAsync(IQueryable<UserDbModel> dbObjects, UsersConvertParams convertParams)
    {
        return [.. (await dbObjects.Include(convertParams).ToListAsync()).Select(UsersMapper.ToEntity)];
    }
}
