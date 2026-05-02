namespace Application.Abstractions.Persistence.Read;

public interface IReadRepository<TId>
    where TId : struct
{
    Task<bool> ExistsByIdAsync(TId id, CancellationToken cancellationToken);
    Task<bool> AnyAsync(CancellationToken cancellationToken);
}
