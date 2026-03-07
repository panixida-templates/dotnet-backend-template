namespace Application.Abstractions.Queries;

public interface IQueryService<TId, TGetByIdDto, TSearchDto, in TSearchParams>
    where TId : struct
    where TGetByIdDto : class
    where TSearchDto : class
    where TSearchParams : BaseSearchParams
{
    Task<TGetByIdDto> GetByIdAsync(TId id, CancellationToken cancellationToken);
    Task<SearchResult<TSearchDto>> SearchAsync(TSearchParams searchParams, CancellationToken cancellationToken);
    Task<bool> ExistsByIdAsync(TId id, CancellationToken cancellationToken);
    Task<bool> AnyAsync(TSearchParams searchParams, CancellationToken cancellationToken);
}
