using Common.SearchParams.Core;

using Domain.Entities.Core;

namespace Application.Abstractions.Persistence.Core;

public interface IWriteRepository<TId, TAggregateRoot, TSearchParams>
    where TId : struct
    where TAggregateRoot : AggregateRoot<TId>
    where TSearchParams : BaseSearchParams
{
    Task<TAggregateRoot> GetAsync(TId id, CancellationToken cancellationToken);
    Task<TId> CreateAsync(TAggregateRoot entity, CancellationToken cancellationToken);
    Task<IReadOnlyList<TId>> CreateAsync(IReadOnlyList<TAggregateRoot> entities, CancellationToken cancellationToken);
    Task UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken);
    Task UpdateAsync(IReadOnlyList<TAggregateRoot> entities, CancellationToken cancellationToken);
    Task DeleteAsync(TId id, CancellationToken cancellationToken);
    Task DeleteAsync(TSearchParams searchParams, CancellationToken cancellationToken);
}
