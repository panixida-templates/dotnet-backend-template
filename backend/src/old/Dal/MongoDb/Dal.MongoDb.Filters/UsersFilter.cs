using System.Text.RegularExpressions;

using Common.SearchParams;

using Dal.MongoDb.DbModels;
using Dal.MongoDb.Filters.Core;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Dal.MongoDb.Filters;

public sealed class UsersFilter : IFilter<Guid, UserDbModel, UsersSearchParams>
{
    public static FilterDefinition<UserDbModel> Filter(FilterDefinition<UserDbModel> filter, FilterDefinitionBuilder<UserDbModel> builder, UsersSearchParams searchParams)
    {
        if (searchParams.Role.HasValue)
        {
            filter = builder.And(filter, builder.Eq(item => item.Role, searchParams.Role.Value));
        }

        if (!string.IsNullOrWhiteSpace(searchParams.SearchQuery))
        {
            var query = searchParams.SearchQuery.Trim();

            var escaped = Regex.Escape(query);
            var regex = new BsonRegularExpression(escaped, "i");

            var searchFilter = builder.Or(
                builder.Regex(item => item.Name, regex),
                builder.Regex(item => item.Email, regex));

            filter = builder.And(filter, searchFilter);
        }

        return filter;
    }
}
