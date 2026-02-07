using Common.SearchParams.Core;

namespace Application.Abstractions.Persistence.Core;

public interface IReadRepository<TId, TDto, TSearchParams>
    where TId : struct
    where TDto : class
    where TSearchParams : BaseSearchParams
{
    Task<TDto> GetAsync(TId id, CancellationToken cancellationToken);
    Task<SearchResult<TDto>> GetAsync(TSearchParams searchParams, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(TSearchParams searchParams, CancellationToken cancellationToken);
}
