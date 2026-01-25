using Bl.Interfaces.Core;

using Common.SearchParams.Core;

using Dal.Interfaces.Core;

using Entities.Core;

namespace Bl.Implementations.Core;

public abstract class BaseBl<TId, TEntity, TSearchParams, TConvertParams>(IBaseDal<TId, TEntity, TSearchParams, TConvertParams> baseDal)
    : IBaseBl<TId, TEntity, TSearchParams, TConvertParams>
    where TId : struct
    where TEntity : BaseEntity<TId>
    where TSearchParams : BaseSearchParams
    where TConvertParams : class, new()
{
    public Task<TEntity> GetAsync(TId id, TConvertParams? convertParams = null)
    {
        return baseDal.GetAsync(id, convertParams);
    }

    public Task<SearchResult<TEntity>> GetAsync(TSearchParams searchParams, TConvertParams? convertParams = null)
    {
        return baseDal.GetAsync(searchParams, convertParams);
    }

    public Task<bool> ExistsAsync(TId id)
    {
        return baseDal.ExistsAsync(id);
    }

    public Task<bool> ExistsAsync(TSearchParams searchParams)
    {
        return baseDal.ExistsAsync(searchParams);
    }

    public Task<TId> AddOrUpdateAsync(TEntity entity)
    {
        return baseDal.AddOrUpdateAsync(entity);
    }

    public Task<IList<TId>> AddOrUpdateAsync(IList<TEntity> entities)
    {
        return baseDal.AddOrUpdateAsync(entities);
    }

    public Task DeleteAsync(TId id)
    {
        return baseDal.DeleteAsync(id);
    }

    public Task DeleteAsync(TSearchParams searchParams)
    {
        return baseDal.DeleteAsync(searchParams);
    }
}
