using Common.SearchParams.Core;

using Entities.Core;

namespace Bl.Interfaces.Core;

public interface IBaseBl<TId, TEntity, TSearchParams, TConvertParams>
    where TId : struct
    where TEntity : BaseEntity<TId>
    where TSearchParams : BaseSearchParams
    where TConvertParams : class, new()
{
    Task<TEntity> GetAsync(TId id, TConvertParams? convertParams = null);
    Task<SearchResult<TEntity>> GetAsync(TSearchParams searchParams, TConvertParams? convertParams = null);
    Task<bool> ExistsAsync(TId id);
    Task<bool> ExistsAsync(TSearchParams searchParams);
    Task<TId> AddOrUpdateAsync(TEntity entity);
    Task<IList<TId>> AddOrUpdateAsync(IList<TEntity> entities);
    Task DeleteAsync(TId id);
    Task DeleteAsync(TSearchParams searchParams);
}
