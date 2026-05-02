using Application.Abstractions.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Ef.Core.Write;

internal abstract class Repository<TDbContext, TId, TAggregateRoot>(
    TDbContext dbContext,
    IAggregateTracker aggregateTracker) : IRepository<TId, TAggregateRoot>
    where TDbContext : DbContext
    where TId : struct
    where TAggregateRoot : AggregateRoot<TId>
{
    protected readonly DbSet<TAggregateRoot> DbSet = dbContext.Set<TAggregateRoot>();
    protected virtual IQueryable<TAggregateRoot> Query => DbSet.AsNoTracking();

    public virtual Task<TAggregateRoot?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken)
    {
        return Query
            .FirstOrDefaultAsync(item => item.Id.Equals(id), cancellationToken);
    }

    public virtual void Add(TAggregateRoot aggregateRoot)
    {
        DbSet.Add(aggregateRoot);
        aggregateTracker.Track(aggregateRoot);
    }

    public virtual void Update(TAggregateRoot aggregateRoot)
    {
        DbSet.Update(aggregateRoot);
        aggregateTracker.Track(aggregateRoot);
    }

    public virtual void Delete(TAggregateRoot aggregateRoot)
    {
        DbSet.Remove(aggregateRoot);
        aggregateTracker.Track(aggregateRoot);
    }
}
