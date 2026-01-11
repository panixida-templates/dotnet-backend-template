using Common.ConvertParams;
using Common.SearchParams;

using Dal.Interfaces;
using Dal.MongoDb.DbModels;
using Dal.MongoDb.Implementations.Core;
using Dal.MongoDb.Mappers;

using Entities;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Dal.MongoDb.Implementations;

public sealed class UsersDal(IMongoDatabase database)
    : BaseDal<Guid, UserDbModel, User, UsersMapper, UsersSearchParams, UsersConvertParams>(database),
      IUsersDal
{
    protected override string CollectionName => "users";

    protected override async ValueTask<FilterDefinition<UserDbModel>> BuildDbFilterAsync(UsersSearchParams searchParams)
    {
        var filter = await base.BuildDbFilterAsync(searchParams);

        if (searchParams.Role.HasValue)
        {
            filter = Builders<UserDbModel>.Filter.And(
                filter,
                Builders<UserDbModel>.Filter.Eq(item => item.Role, searchParams.Role.Value));
        }

        if (!string.IsNullOrWhiteSpace(searchParams.SearchQuery))
        {
            var query = searchParams.SearchQuery.Trim();
            var regex = new BsonRegularExpression(query, "i");

            var textFilter = Builders<UserDbModel>.Filter.Or(
                Builders<UserDbModel>.Filter.Regex(item => item.Name, regex),
                Builders<UserDbModel>.Filter.Regex(item => item.Email, regex));

            filter = Builders<UserDbModel>.Filter.And(filter, textFilter);
        }

        return filter;
    }

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
