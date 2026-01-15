using Common.SearchParams.Core;

using Dal.Ef.DbModels.Core;

namespace Dal.Ef.Filters.Core;

public interface IFilter<TId, TDbModel, TSearchParams>
    where TId : struct
    where TDbModel : BaseDbModel<TId>
    where TSearchParams : BaseSearchParams
{
    static abstract IQueryable<TDbModel> Filter(IQueryable<TDbModel> dbObjects, TSearchParams searchParams);
}
