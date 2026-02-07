using Application.Abstractions.Persistence.Core;

using Common.Exceptions;
using Common.SearchParams.Core;

using Domain.Entities.Core;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Ef.Core;

internal abstract class EfWriteRepository<TDbContext, TId, TDbModel, TAggregateRoot, TSearchParams, TMapper, TFilter>(TDbContext dbContext)
    : IWriteRepository<TId, TAggregateRoot, TSearchParams>
    where TDbContext : DbContext
    where TId : struct
    where TDbModel : DbModel<TId>, new()
    where TAggregateRoot : AggregateRoot<TId>
    where TSearchParams : BaseSearchParams
    where TMapper : IWriteMapper<TId, TDbModel, TAggregateRoot>
    where TFilter : IFilter<TId, TDbModel, TSearchParams>
{
    public virtual async Task<TAggregateRoot> GetAsync(TId id, CancellationToken cancellationToken)
    {
        var dbObjects = dbContext.Set<TDbModel>().AsQueryable();
        dbObjects = EfFilters<TId, TDbModel>.BuildDbNotDeletedFilter(dbObjects, id);

        var dbObject = await dbObjects.FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"{typeof(TDbModel).Name} с id={id} не найдена");

        var entity = TMapper.ToEntity(dbObject);

        return entity;
    }

    public virtual async Task<TId> CreateAsync(TAggregateRoot entity, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var dbObject = CreateDbObject(entity, now);
        dbContext.Set<TDbModel>().Add(dbObject);
        return dbObject.Id;
    }

    public virtual async Task<IReadOnlyList<TId>> CreateAsync(IReadOnlyList<TAggregateRoot> entities, CancellationToken cancellationToken)
    {
        if (entities.Count == 0)
        {
            return [];
        }

        var now = DateTime.UtcNow;
        var dbObjects = CreateDbObjects(entities, now);
        dbContext.Set<TDbModel>().AddRange(dbObjects);

        return [.. dbObjects.Select(item => item.Id)];
    }

    public virtual async Task UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken)
    {
        var dbSet = dbContext.Set<TDbModel>();

        var query = dbSet.AsQueryable();
        query = EfFilters<TId, TDbModel>.BuildDbNotDeletedFilter(query, entity.Id);

        var dbObject = await query.FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"{typeof(TDbModel).Name} с id={entity.Id} не найдена");

        TMapper.ToDbModel(entity, dbObject);
        dbObject.UpdatedAt = DateTime.UtcNow;
    }

    public virtual async Task UpdateAsync(IReadOnlyList<TAggregateRoot> entities, CancellationToken cancellationToken)
    {
        if (entities.Count == 0)
        {
            return;
        }

        var ids = entities.Select(item => item.Id).Distinct().ToArray();

        var dbSet = dbContext.Set<TDbModel>();
        var query = dbSet.AsQueryable().Where(item => !item.DeletedAt.HasValue);
        query = query.Where(item => ids.Contains(item.Id));

        var existingById = await query.ToDictionaryAsync(item => item.Id, cancellationToken);
        if (existingById.Count != ids.Length)
        {
            var missingIds = ids.Where(id => !existingById.ContainsKey(id)).ToArray();
            throw new NotFoundException($"{typeof(TDbModel).Name} не найдены: {string.Join(", ", missingIds)}");
        }

        var now = DateTime.UtcNow;

        foreach (var entity in entities)
        {
            var dbObject = existingById[entity.Id];
            TMapper.ToDbModel(entity, dbObject);
            dbObject.UpdatedAt = now;
        }
    }

    public virtual Task DeleteAsync(TId id, CancellationToken cancellationToken)
    {
        var dbObjects = dbContext.Set<TDbModel>().AsQueryable();
        dbObjects = EfFilters<TId, TDbModel>.BuildDbNotDeletedFilter(dbObjects, id);
        return SoftDeleteAsync(dbObjects, cancellationToken);
    }

    public virtual Task DeleteAsync(TSearchParams searchParams, CancellationToken cancellationToken)
    {
        var dbObjects = dbContext.Set<TDbModel>().AsQueryable();
        dbObjects = EfFilters<TId, TDbModel>.BuildDbFilter<TSearchParams, TFilter>(dbObjects, searchParams);
        return SoftDeleteAsync(dbObjects, cancellationToken);
    }

    private static List<TDbModel> CreateDbObjects(IReadOnlyList<TAggregateRoot> entities, DateTime now)
    {
        var dbObjects = new List<TDbModel>(entities.Count);

        foreach (var entity in entities)
        {
            var dbObject = CreateDbObject(entity, now);
            dbObjects.Add(dbObject);
        }

        return dbObjects;
    }

    private static TDbModel CreateDbObject(TAggregateRoot entity, DateTime now)
    {
        var dbObject = new TDbModel();
        TMapper.ToDbModel(entity, dbObject);

        ApplyCreateAudit(dbObject, now);

        return dbObject;
    }

    private static void ApplyCreateAudit(TDbModel dbObject, DateTime now)
    {
        dbObject.CreatedAt = now;
        dbObject.UpdatedAt = now;
        dbObject.DeletedAt = null;
    }

    private static async Task SoftDeleteAsync(IQueryable<TDbModel> dbObjects, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        await dbObjects.ExecuteUpdateAsync(item =>
        {
            item
                .SetProperty(property => property.UpdatedAt, now)
                .SetProperty(property => property.DeletedAt, now);
        }, cancellationToken);
    }
}
