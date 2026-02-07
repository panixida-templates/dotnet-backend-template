using Common.SearchParams.Core;

namespace Infrastructure.Persistence.Ef.Core;

internal static class EfFilters<TId, TDbModel>
    where TId : struct
    where TDbModel : DbModel<TId>
{
    internal static IQueryable<TDbModel> BuildDbNotDeletedFilter(IQueryable<TDbModel> dbObjects, TId id)
    {
        return dbObjects.Where(item => item.Id.Equals(id) && !item.DeletedAt.HasValue);
    }

    internal static IQueryable<TDbModel> BuildDbFilter<TSearchParams, TFilter>(IQueryable<TDbModel> dbObjects, TSearchParams searchParams)
        where TSearchParams : BaseSearchParams
        where TFilter : IFilter<TId, TDbModel, TSearchParams>
    {
        if (searchParams.IsDeleted)
        {
            dbObjects = dbObjects.Where(item => item.DeletedAt.HasValue);
        }
        else
        {
            dbObjects = dbObjects.Where(item => !item.DeletedAt.HasValue);
        }

        if (searchParams.CreatedFrom.HasValue)
        {
            dbObjects = dbObjects.Where(item => searchParams.CreatedFrom <= item.CreatedAt);
        }
        if (searchParams.CreatedTo.HasValue)
        {
            dbObjects = dbObjects.Where(item => item.CreatedAt <= searchParams.CreatedTo);
        }
        if (searchParams.UpdatedFrom.HasValue)
        {
            dbObjects = dbObjects.Where(item => searchParams.UpdatedFrom <= item.UpdatedAt);
        }
        if (searchParams.UpdatedTo.HasValue)
        {
            dbObjects = dbObjects.Where(item => item.UpdatedAt <= searchParams.UpdatedTo);
        }
        if (searchParams.DeletedFrom.HasValue)
        {
            dbObjects = dbObjects.Where(item => searchParams.DeletedFrom <= item.DeletedAt);
        }
        if (searchParams.DeletedTo.HasValue)
        {
            dbObjects = dbObjects.Where(item => item.DeletedAt <= searchParams.DeletedTo);
        }

        dbObjects = TFilter.Filter(dbObjects, searchParams);

        return dbObjects;
    }
}
