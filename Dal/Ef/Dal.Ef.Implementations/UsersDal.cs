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

public sealed class UsersDal(DefaultDbContext context) : 
    BaseDal<DefaultDbContext, Guid, UserDbModel, User, UsersMapper, UsersSearchParams, UsersConvertParams>(context),
    IUsersDal
{
    protected override async ValueTask<IQueryable<UserDbModel>> BuildDbFilterAsync(IQueryable<UserDbModel> dbObjects, UsersSearchParams searchParams)
    {
        dbObjects = await base.BuildDbFilterAsync(dbObjects, searchParams);
        return dbObjects.Filter(searchParams);
    }

    protected override async Task<IList<User>> BuildEntitiesListAsync(IQueryable<UserDbModel> dbObjects, UsersConvertParams convertParams)
    {
        return [.. (await dbObjects.Include(convertParams).ToListAsync()).Select(UsersMapper.ToEntity)];
    }
}
