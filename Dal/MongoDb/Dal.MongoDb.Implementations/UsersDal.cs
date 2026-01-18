using Common.ConvertParams;
using Common.SearchParams;

using Dal.Interfaces;

using Dal.MongoDb.DbModels;
using Dal.MongoDb.Filters;
using Dal.MongoDb.Implementations.Core;
using Dal.MongoDb.Mappers;

using Entities;

using MongoDB.Driver;

namespace Dal.MongoDb.Implementations;

public sealed class UsersDal(IMongoDatabase database)
    : BaseDal<Guid, UserDbModel, User, UsersSearchParams, UsersConvertParams, UsersMapper, UsersFilter>(database, "users"),
      IUsersDal
{
    protected override Task<IList<User>> BuildEntitiesListAsync(IReadOnlyList<UserDbModel> dbObjects, UsersConvertParams convertParams)
    {
        var result = new List<User>(dbObjects.Count);

        foreach (var dbObject in dbObjects)
        {
            result.Add(UsersMapper.ToEntity(dbObject));
        }

        return Task.FromResult<IList<User>>(result);
    }
}
