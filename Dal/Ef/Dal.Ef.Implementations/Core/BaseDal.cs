using Common.Enums;
using Common.Exceptions;
using Common.SearchParams.Core;

using Dal.Ef.DbModels.Core;
using Dal.Ef.Filters.Core;
using Dal.Ef.Includes.Core;
using Dal.Ef.Mappers.Core;

using Dal.Interfaces.Core;

using Entities.Core;

using Microsoft.EntityFrameworkCore;

using System.Linq.Dynamic.Core;

namespace Dal.Ef.Implementations.Core;

public abstract class BaseDal<TDbContext, TId, TDbModel, TEntity, TSearchParams, TConvertParams, TMapper, TFilter, TInclude>(TDbContext dbContext)
    : IBaseDal<TId, TEntity, TSearchParams, TConvertParams>
    where TDbContext : DbContext
    where TId : struct
    where TDbModel : BaseDbModel<TId>, new()
    where TEntity : BaseEntity<TId>
    where TSearchParams : BaseSearchParams
    where TConvertParams : class, new()
    where TMapper : IMapper<TId, TDbModel, TEntity>
    where TFilter : IFilter<TId, TDbModel, TSearchParams>
    where TInclude : IInclude<TId, TDbModel, TConvertParams>
{
    public virtual async Task<TEntity> GetAsync(TId id, TConvertParams? convertParams = null)
    {
        convertParams ??= new TConvertParams();

        var dbObjects = dbContext.Set<TDbModel>().AsNoTrackingWithIdentityResolution();
        dbObjects = BuildDbNotDeletedFilter(dbObjects, id);
        dbObjects = dbObjects.Take(1);

        var entity = (await BuildEntitiesListAsync(dbObjects, convertParams)).FirstOrDefault()
            ?? throw new NotFoundException($"{typeof(TDbModel).Name} с id={id} не найдена");

        return entity;
    }

    public virtual async Task<SearchResult<TEntity>> GetAsync(TSearchParams searchParams, TConvertParams? convertParams = null)
    {
        ArgumentNullException.ThrowIfNull(searchParams);
        convertParams ??= new TConvertParams();

        var dbObjects = dbContext.Set<TDbModel>().AsNoTrackingWithIdentityResolution();
        dbObjects = BuildDbFilter(dbObjects, searchParams);
        dbObjects = BuildDbSort(dbObjects, searchParams);

        var searchResult = new SearchResult<TEntity>
        {
            Total = await dbObjects.CountAsync(),
            Objects = [],
            RequestedObjectsCount = searchParams.ObjectsCount,
            RequestedPage = searchParams.Page,
        };

        dbObjects = BuildDbPagination(dbObjects, searchParams);
        searchResult.Objects = await BuildEntitiesListAsync(dbObjects, convertParams);

        return searchResult;
    }

    public virtual Task<bool> ExistsAsync(TId id)
    {
        var dbObjects = dbContext.Set<TDbModel>().AsNoTracking();
        dbObjects = BuildDbNotDeletedFilter(dbObjects, id);
        return dbObjects.AnyAsync();
    }

    public virtual Task<bool> ExistsAsync(TSearchParams searchParams)
    {
        ArgumentNullException.ThrowIfNull(searchParams);

        var data = dbContext;
        var dbObjects = data.Set<TDbModel>().AsNoTracking();

        return BuildDbFilter(dbObjects, searchParams).AnyAsync();
    }

    public virtual async Task<TId> AddOrUpdateAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var dbObjects = dbContext.Set<TDbModel>();
        var dbObject = await dbObjects.FirstOrDefaultAsync(item => item.Id.Equals(entity.Id));

        var now = DateTime.UtcNow;
        dbObject = AddOrUpdate(dbObjects, entity, dbObject, now);

        await dbContext.SaveChangesAsync();

        entity.Id = dbObject.Id;

        return dbObject.Id;
    }

    public virtual async Task<IList<TId>> AddOrUpdateAsync(IList<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var dbObjects = dbContext.Set<TDbModel>();

        var ids = entities.Select(item => item.Id).Distinct().ToArray();
        var existingById = await dbObjects.Where(item => ids.Contains(item.Id)).ToDictionaryAsync(item => item.Id);

        var savedDbObjects = new List<TDbModel>(entities.Count);
        var now = DateTime.UtcNow;

        foreach (var entity in entities)
        {
            existingById.TryGetValue(entity.Id, out var dbObject);
            dbObject = AddOrUpdate(dbObjects, entity, dbObject, now);
            savedDbObjects.Add(dbObject);
        }

        await dbContext.SaveChangesAsync();

        for (int i = 0; i < savedDbObjects.Count; i++)
        {
            entities[i].Id = savedDbObjects[i].Id;
        }

        return [.. savedDbObjects.Select(item => item.Id)];
    }

    public virtual Task DeleteAsync(TId id)
    {
        var dbObjects = dbContext.Set<TDbModel>().AsQueryable();
        dbObjects = BuildDbNotDeletedFilter(dbObjects, id);
        return SoftDeleteAsync(dbObjects);
    }

    public virtual Task DeleteAsync(TSearchParams searchParams)
    {
        ArgumentNullException.ThrowIfNull(searchParams);

        var dbObjects = dbContext.Set<TDbModel>().AsQueryable();
        dbObjects = BuildDbFilter(dbObjects, searchParams);

        return SoftDeleteAsync(dbObjects);
    }

    private static IQueryable<TDbModel> BuildDbNotDeletedFilter(IQueryable<TDbModel> dbObjects, TId id)
    {
        return dbObjects.Where(item => item.Id.Equals(id) && !item.DeletedAt.HasValue);
    }

    private static IQueryable<TDbModel> BuildDbFilter(IQueryable<TDbModel> dbObjects, TSearchParams searchParams)
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

        dbObjects = TFilter.Filter(dbObjects, searchParams);

        return dbObjects;
    }

    private static IQueryable<TDbModel> BuildDbSort(IQueryable<TDbModel> dbObjects, TSearchParams searchParams)
    {
        if (!string.IsNullOrEmpty(searchParams.SortField))
        {
            if (searchParams.SortOrder == SortOrder.Descending)
            {
                dbObjects = dbObjects.OrderBy($"{searchParams.SortField} descending");
            }
            else
            {
                dbObjects = dbObjects.OrderBy(searchParams.SortField);
            }
        }
        else
        {
            var visitor = new OrderedQueryableVisitor();
            visitor.Visit(dbObjects.Expression);

            if (visitor.IsOrdered && dbObjects is IOrderedQueryable<TDbModel> orderedObjects)
            {
                dbObjects = orderedObjects
                    .ThenByDescending(item => item.UpdatedAt)
                    .ThenByDescending(item => item.CreatedAt)
                    .ThenByDescending(item => item.Id);
            }
            else
            {
                dbObjects = dbObjects
                    .OrderByDescending(item => item.UpdatedAt)
                    .ThenByDescending(item => item.CreatedAt)
                    .ThenByDescending(item => item.Id);
            }
        }

        return dbObjects;
    }

    private static IQueryable<TDbModel> BuildDbPagination(IQueryable<TDbModel> dbObjects, TSearchParams searchParams)
    {
        if (searchParams.ObjectsCount == 0)
        {
            return dbObjects.Take(0);
        }

        dbObjects = dbObjects.Skip((searchParams.Page - 1) * searchParams.ObjectsCount ?? 0);

        if (searchParams.ObjectsCount.HasValue)
        {
            dbObjects = dbObjects.Take(searchParams.ObjectsCount.Value);
        }

        return dbObjects;
    }

    private static async Task<IList<TEntity>> BuildEntitiesListAsync(IQueryable<TDbModel> dbObjects, TConvertParams convertParams)
    {
        dbObjects = TInclude.Include(dbObjects, convertParams);
        var entities = (await dbObjects.ToListAsync()).Select(TMapper.ToEntity).ToList();
        return entities;
    }

    private static TDbModel AddOrUpdate(DbSet<TDbModel> dbObjects, TEntity entity, TDbModel? dbObject, DateTime now)
    {
        var exists = dbObject != null;
        dbObject ??= new TDbModel();

        TMapper.ToDbModel(entity, dbObject);

        if (!exists)
        {
            dbObject.CreatedAt = now;
            dbObjects.Add(dbObject);
        }

        dbObject.UpdatedAt = now;

        return dbObject;
    }

    private static async Task SoftDeleteAsync(IQueryable<TDbModel> dbObjects)
    {
        var now = DateTime.UtcNow;

        await dbObjects.ExecuteUpdateAsync(item =>
        {
            item
                .SetProperty(property => property.UpdatedAt, now)
                .SetProperty(property => property.DeletedAt, now);
        });
    }
}
