using Application.Abstractions.Queries;

namespace Infrastructure.Persistence.Ef.Core;

internal interface IFilter<TId, TDbModel, TSearchParams>
    where TId : struct
    where TDbModel : DbModel<TId>
    where TSearchParams : BaseSearchParams
{
    static abstract IQueryable<TDbModel> Filter(IQueryable<TDbModel> dbObjects, TSearchParams searchParams);
}
