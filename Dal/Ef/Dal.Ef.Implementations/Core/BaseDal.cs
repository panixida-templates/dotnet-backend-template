using System.Linq.Dynamic.Core;

using Common.Enums;
using Common.Exceptions;
using Common.SearchParams.Core;

using Dal.Ef.DbModels.Core;
using Dal.Ef.Mappers.Core;
using Dal.Interfaces.Core;

using Entities.Core;

using Microsoft.EntityFrameworkCore;

namespace Dal.Ef.Implementations.Core;

public abstract class BaseDal<TDbContext, TId, TDbModel, TEntity, TMapper, TSearchParams, TConvertParams>(TDbContext dbContext)
    : IBaseDal<TId, TEntity, TSearchParams, TConvertParams>
    where TDbContext : DbContext
    where TId : struct
    where TDbModel : BaseDbModel<TId>, new()
    where TEntity : BaseEntity<TId>
    where TMapper : IMapper<TDbModel, TEntity>
    where TSearchParams : BaseSearchParams
    where TConvertParams : class, new()
{
    public virtual async Task<TEntity> GetAsync(TId id, TConvertParams? convertParams = null)
    {
        convertParams ??= new TConvertParams();

        var dbObject = dbContext.Set<TDbModel>()
            .AsNoTracking()
            .Where(item => item.Id.Equals(id) && !item.DeletedAt.HasValue)
            .Take(1);

        var entity = (await BuildEntitiesListAsync(dbObject, convertParams)).FirstOrDefault()
            ?? throw new NotFoundException($"{typeof(TDbModel).Name} с id={id} не найдена");

        return entity;
    }

    public virtual async Task<SearchResult<TEntity>> GetAsync(TSearchParams searchParams, TConvertParams? convertParams = null)
    {
        ArgumentNullException.ThrowIfNull(searchParams);
        convertParams ??= new TConvertParams();

        var dbObjects = dbContext.Set<TDbModel>().AsNoTracking();
        dbObjects = await BuildDbFilterAsync(dbObjects, searchParams);
        dbObjects = await BuildSortAsync(dbObjects, searchParams);

        var searchResult = new SearchResult<TEntity>
        {
            Total = await dbObjects.CountAsync(),
            Objects = [],
            RequestedObjectsCount = searchParams.ObjectsCount,
            RequestedPage = searchParams.Page,
        };

        if (searchParams.ObjectsCount == 0)
        {
            return searchResult;
        }

        dbObjects = dbObjects.Skip((searchParams.Page - 1) * (searchParams.ObjectsCount ?? 0));
        if (searchParams.ObjectsCount.HasValue)
        {
            dbObjects = dbObjects.Take(searchParams.ObjectsCount.Value);
        }
        searchResult.Objects = await BuildEntitiesListAsync(dbObjects, convertParams);

        return searchResult;
    }

    public virtual Task<bool> ExistsAsync(TId id)
    {
        return dbContext.Set<TDbModel>()
            .AsNoTracking()
            .Where(item => item.Id.Equals(id) && !item.DeletedAt.HasValue)
            .AnyAsync();
    }

    public virtual async Task<bool> ExistsAsync(TSearchParams searchParams)
    {
        ArgumentNullException.ThrowIfNull(searchParams);

        var data = dbContext;
        var objects = data.Set<TDbModel>().AsNoTracking();

        return await (await BuildDbFilterAsync(objects, searchParams)).AnyAsync();
    }

    public virtual async Task<TId> AddOrUpdateAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var data = dbContext;
        var objects = data.Set<TDbModel>();
        var dbObject = await objects.FirstOrDefaultAsync(item => item.Id.Equals(entity.Id));
        var exists = dbObject != null;
        dbObject = TMapper.ToDbModel(entity);

        var now = DateTime.UtcNow;
        if (!exists)
        {
            dbObject.CreatedAt = now;
            objects.Add(dbObject);
        }
        dbObject.UpdatedAt = now;

        await data.SaveChangesAsync();

        entity.Id = dbObject.Id;

        return dbObject.Id;
    }

    public virtual async Task<IList<TId>> AddOrUpdateAsync(IList<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var data = dbContext;
        var entitiesIdArray = entities.Select(item => item.Id).ToArray();
        var dbSet = data.Set<TDbModel>();
        var dbObjectsDictionary = await dbSet.Where(item => entitiesIdArray.Any(id => item.Id.Equals(id))).ToDictionaryAsync(item => item.Id);

        var existingSet = new HashSet<TId>();
        var dbObjects = new List<TDbModel>();
        var addedObjects = new List<TDbModel>();

        foreach (var entity in entities)
        {
            var id = entity.Id;
            var exists = dbObjectsDictionary.TryGetValue(id, out var dbObject);
            dbObject = TMapper.ToDbModel(entity);

            if (exists)
            {
                existingSet.Add(id);
            }

            var now = DateTime.UtcNow;
            if (!exists)
            {
                dbObject.CreatedAt = now;
            }
            dbObject.UpdatedAt = now;

            dbObjects.Add(dbObject);

            if (!exists)
            {
                addedObjects.Add(dbObject);
            }
        }

        dbSet.AddRange(addedObjects);

        await data.SaveChangesAsync();

        return [.. dbObjects.Select(item => item.Id)];
    }

    public virtual Task DeleteAsync(TId id)
    {
        var dbObject = dbContext.Set<TDbModel>().Where(item => item.Id.Equals(id) && !item.DeletedAt.HasValue);
        return SoftDeleteAsync(dbObject);
    }

    public virtual async Task DeleteAsync(TSearchParams searchParams)
    {
        ArgumentNullException.ThrowIfNull(searchParams);

        var dbObjects = dbContext.Set<TDbModel>().AsQueryable();
        dbObjects = await BuildDbFilterAsync(dbObjects, searchParams);

        await SoftDeleteAsync(dbObjects);
    }

    protected virtual ValueTask<IQueryable<TDbModel>> BuildDbFilterAsync(IQueryable<TDbModel> dbObjects, TSearchParams searchParams)
    {
        if (searchParams.IsDeleted)
        {
            dbObjects = dbObjects.Where(item => item.DeletedAt.HasValue);
        }
        else
        {
            dbObjects = dbObjects.Where(item => !item.DeletedAt.HasValue);
        }

        if (searchParams.CreatedFrom.HasValue)
        {
            dbObjects = dbObjects.Where(item => searchParams.CreatedFrom <= item.CreatedAt);
        }
        if (searchParams.CreatedTo.HasValue)
        {
            dbObjects = dbObjects.Where(item => item.CreatedAt <= searchParams.CreatedTo);
        }
        if (searchParams.UpdatedFrom.HasValue)
        {
            dbObjects = dbObjects.Where(item => searchParams.UpdatedFrom <= item.UpdatedAt);
        }
        if (searchParams.UpdatedTo.HasValue)
        {
            dbObjects = dbObjects.Where(item => item.UpdatedAt <= searchParams.UpdatedTo);
        }
        if (searchParams.DeletedFrom.HasValue)
        {
            dbObjects = dbObjects.Where(item => searchParams.DeletedFrom <= item.DeletedAt);
        }
        if (searchParams.DeletedTo.HasValue)
        {
            dbObjects = dbObjects.Where(item => item.DeletedAt <= searchParams.DeletedTo);
        }

        return ValueTask.FromResult(dbObjects);
    }

    protected virtual ValueTask<IQueryable<TDbModel>> BuildSortAsync(IQueryable<TDbModel> objects, TSearchParams searchParams)
    {
        if (!string.IsNullOrEmpty(searchParams.SortField))
        {
            if (searchParams.SortOrder == SortOrder.Descending)
            {
                objects = objects.OrderBy($"{searchParams.SortField} descending");
            }
            else
            {
                objects = objects.OrderBy(searchParams.SortField);
            }
        }
        else
        {
            var visitor = new OrderedQueryableVisitor();
            visitor.Visit(objects.Expression);

            if (visitor.IsOrdered && objects is IOrderedQueryable<TDbModel> orderedObjects)
            {
                objects = orderedObjects
                    .ThenByDescending(item => item.UpdatedAt)
                    .ThenByDescending(item => item.CreatedAt)
                    .ThenByDescending(item => item.Id);
            }
            else
            {
                objects = objects
                    .OrderByDescending(item => item.UpdatedAt)
                    .ThenByDescending(item => item.CreatedAt)
                    .ThenByDescending(item => item.Id);
            }
        }

        return ValueTask.FromResult(objects);
    }

    protected abstract Task<IList<TEntity>> BuildEntitiesListAsync(IQueryable<TDbModel> dbObjects, TConvertParams convertParams);

    private static async Task SoftDeleteAsync(IQueryable<TDbModel> dbObjects)
    {
        var now = DateTime.UtcNow;

        await dbObjects.ExecuteUpdateAsync(item =>
        {
            item
                .SetProperty(property => property.DeletedAt, now)
                .SetProperty(property => property.UpdatedAt, now);
        });
    }
}