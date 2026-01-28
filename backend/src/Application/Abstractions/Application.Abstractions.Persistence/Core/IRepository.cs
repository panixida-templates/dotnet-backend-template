using Common.SearchParams.Core;

using Domain.Entities.Core;

namespace Application.Abstractions.Persistence.Core;

public interface IRepository<TId, TEntity, TSearchParams, TConvertParams>
    where TId : struct
    where TEntity : Entity<TId>
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
