using Application.Abstractions.Persistence;

using Domain.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Persistence.Ef.Core;

internal abstract class Repository<TDbContext, TId, TDbModel, TAggregateRoot, TMapper>(
    TDbContext dbContext,
    IAggregateTracker aggregateTracker) : IRepository<TId, TAggregateRoot>
    where TDbContext : DbContext
    where TId : struct
    where TDbModel : DbModel<TId>, new()
    where TAggregateRoot : class, IAggregateRoot
    where TMapper : IEntityMapper<TId, TDbModel, TAggregateRoot>
{
    public virtual async Task<TAggregateRoot?> GetByIdAsync(TId id, CancellationToken cancellationToken)
    {
        var dbObjects = dbContext.Set<TDbModel>().AsNoTracking();
        dbObjects = QueryableExtensions<TId, TDbModel>.ApplyGetByIdFilter(dbObjects, id);

        var dbObject = await dbObjects.FirstOrDefaultAsync(cancellationToken);
        if (dbObject is null)
        {
            return null;
        }

        return TMapper.ToEntity(dbObject);
    }

    public virtual void Add(TAggregateRoot aggregateRoot)
    {
        var dbObject = new TDbModel();
        TMapper.ToDbModel(aggregateRoot, dbObject);

        if (dbObject is AuditableDbModel<TId> auditableDbObject)
        {
            ApplyCreateAudit(auditableDbObject);
        }

        dbContext.Set<TDbModel>().Add(dbObject);

        aggregateTracker.Track(aggregateRoot);
    }

    public virtual void Update(TAggregateRoot aggregateRoot)
    {
        var dbObject = new TDbModel();
        TMapper.ToDbModel(aggregateRoot, dbObject);

        if (dbObject is AuditableDbModel<TId> auditableDbObject)
        {
            ApplyUpdateAudit(auditableDbObject);

            var entry = dbContext.Attach(auditableDbObject);
            entry.State = EntityState.Modified;

            ApplyUpdateEntryRules(entry);
        }
        else
        {
            dbContext.Attach(dbObject);

            var entry = dbContext.Entry(dbObject);
            entry.State = EntityState.Modified;
        }

        aggregateTracker.Track(aggregateRoot);
    }

    public virtual void Delete(TAggregateRoot aggregateRoot)
    {
        var dbObject = new TDbModel();
        TMapper.ToDbModel(aggregateRoot, dbObject);

        if (dbObject is AuditableDbModel<TId> auditableDbObject)
        {
            ApplySoftDeleteAudit(auditableDbObject);

            var entry = dbContext.Attach(auditableDbObject);
            entry.State = EntityState.Unchanged;

            ApplySoftDeleteEntryRules(entry);
        }
        else
        {
            dbContext.Remove(dbObject);
        }

        aggregateTracker.Track(aggregateRoot);
    }

    private static void ApplyCreateAudit(AuditableDbModel<TId> dbObject)
    {
        var now = DateTime.UtcNow;

        dbObject.CreatedAt = now;
        dbObject.UpdatedAt = now;
        dbObject.DeletedAt = null;
    }

    private static void ApplyUpdateAudit(AuditableDbModel<TId> dbObject)
    {
        dbObject.UpdatedAt = DateTime.UtcNow;
    }

    private static void ApplyUpdateEntryRules(EntityEntry<AuditableDbModel<TId>> entry)
    {
        entry.Property(item => item.CreatedAt).IsModified = false;
        entry.Property(item => item.DeletedAt).IsModified = false;
    }

    private static void ApplySoftDeleteAudit(AuditableDbModel<TId> dbObject)
    {
        var now = DateTime.UtcNow;

        dbObject.UpdatedAt = now;
        dbObject.DeletedAt = now;
    }

    private static void ApplySoftDeleteEntryRules(EntityEntry<AuditableDbModel<TId>> entry)
    {
        entry.Property(item => item.UpdatedAt).IsModified = true;
        entry.Property(item => item.DeletedAt).IsModified = true;
    }
}
