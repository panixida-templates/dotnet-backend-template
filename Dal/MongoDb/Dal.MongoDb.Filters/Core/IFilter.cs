using Common.SearchParams.Core;

using Dal.MongoDb.DbModels.Core;

using MongoDB.Driver;

namespace Dal.MongoDb.Filters.Core;

public interface IFilter<TId, TDbModel, TSearchParams>
    where TId : struct
    where TDbModel : BaseDbModel<TId>
    where TSearchParams : BaseSearchParams
{
    static abstract FilterDefinition<TDbModel> Filter(FilterDefinition<TDbModel> filter, FilterDefinitionBuilder<TDbModel> builder, TSearchParams searchParams);
}
