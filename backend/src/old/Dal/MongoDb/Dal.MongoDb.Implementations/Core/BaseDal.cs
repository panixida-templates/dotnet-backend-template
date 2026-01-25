using Common.Enums;
using Common.Exceptions;
using Common.SearchParams.Core;

using Dal.Interfaces.Core;

using Dal.MongoDb.DbModels.Core;
using Dal.MongoDb.Filters.Core;
using Dal.MongoDb.Mappers.Core;

using Entities.Core;

using MongoDB.Driver;

using System.Diagnostics.CodeAnalysis;

namespace Dal.MongoDb.Implementations.Core;

public abstract class BaseDal<TId, TDbModel, TEntity, TSearchParams, TConvertParams, TMapper, TFilter>(IMongoDatabase database, string collectionName)
    : IBaseDal<TId, TEntity, TSearchParams, TConvertParams>
    where TId : struct
    where TDbModel : BaseDbModel<TId>, new()
    where TEntity : BaseEntity<TId>
    where TSearchParams : BaseSearchParams
    where TConvertParams : class, new()
    where TMapper : IMapper<TId, TDbModel, TEntity>
    where TFilter : IFilter<TId, TDbModel, TSearchParams>
{
    protected IMongoCollection<TDbModel> Collection { get; } = database.GetCollection<TDbModel>(collectionName);

    public virtual async Task<TEntity> GetAsync(TId id, TConvertParams? convertParams = null)
    {
        convertParams ??= new TConvertParams();

        var filter = BuildDbNotDeletedFilter(id);
        var dbObjects = Collection.Find(filter).Limit(1);

        var entity = (await BuildEntitiesListAsync(dbObjects, convertParams)).FirstOrDefault()
            ?? throw new NotFoundException($"{typeof(TDbModel).Name} с id={id} не найдена");

        return entity;
    }

    public virtual async Task<SearchResult<TEntity>> GetAsync(TSearchParams searchParams, TConvertParams? convertParams = null)
    {
        ArgumentNullException.ThrowIfNull(searchParams);
        convertParams ??= new TConvertParams();

        var filter = BuildDbFilter(searchParams);
        var sort = BuildDbSort(searchParams);

        var searchResult = new SearchResult<TEntity>
        {
            Total = await Collection.CountDocumentsAsync(filter),
            Objects = [],
            RequestedObjectsCount = searchParams.ObjectsCount,
            RequestedPage = searchParams.Page,
        };

        var dbObjects = Collection.Find(filter).Sort(sort);
        dbObjects = BuildDbPagination(dbObjects, searchParams);

        searchResult.Objects = await BuildEntitiesListAsync(dbObjects, convertParams);

        return searchResult;
    }

    public virtual Task<bool> ExistsAsync(TId id)
    {
        var filter = BuildDbNotDeletedFilter(id);
        return Collection.Find(filter).AnyAsync();
    }

    public virtual Task<bool> ExistsAsync(TSearchParams searchParams)
    {
        ArgumentNullException.ThrowIfNull(searchParams);
        var filter = BuildDbFilter(searchParams);
        return Collection.Find(filter).AnyAsync();
    }

    public virtual async Task<TId> AddOrUpdateAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var filter = Builders<TDbModel>.Filter.Eq(item => item.Id, entity.Id);
        var dbObject = await Collection.Find(filter).Limit(1).FirstOrDefaultAsync();

        var exists = dbObject != null;
        var now = DateTime.UtcNow;

        dbObject = AddOrUpdate(entity, dbObject, exists, now);

        if (!exists)
        {
            await Collection.InsertOneAsync(dbObject);
        }
        else
        {
            await Collection.ReplaceOneAsync(filter, dbObject, new ReplaceOptions { IsUpsert = true });
        }
        entity.Id = dbObject.Id;

        return dbObject.Id;
    }

    public virtual async Task<IList<TId>> AddOrUpdateAsync(IList<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var ids = entities.Select(item => item.Id).Distinct().ToArray();
        var existingFilter = Builders<TDbModel>.Filter.In(item => item.Id, ids);
        var existingById = (await Collection.Find(existingFilter).ToListAsync()).ToDictionary(item => item.Id);

        var now = DateTime.UtcNow;

        var savedDbObjects = new List<TDbModel>(entities.Count);
        var insertedDbObjects = new List<TDbModel>(entities.Count);
        var updatedDbObjects = new List<WriteModel<TDbModel>>(entities.Count);

        foreach (var entity in entities)
        {
            existingById.TryGetValue(entity.Id, out var dbObject);
            var exists = dbObject != null;

            dbObject = AddOrUpdate(entity, dbObject, exists, now);
            savedDbObjects.Add(dbObject);

            if (!exists)
            {
                insertedDbObjects.Add(dbObject);
            }
            else
            {
                var filter = Builders<TDbModel>.Filter.Eq(item => item.Id, entity.Id);
                updatedDbObjects.Add(new ReplaceOneModel<TDbModel>(filter, dbObject) { IsUpsert = true });
            }
        }

        if (insertedDbObjects.Count > 0)
        {
            await Collection.InsertManyAsync(insertedDbObjects, new InsertManyOptions { IsOrdered = false });
        }
        if (updatedDbObjects.Count > 0)
        {
            await Collection.BulkWriteAsync(updatedDbObjects, new BulkWriteOptions { IsOrdered = false });
        }

        for (int i = 0; i < savedDbObjects.Count; i++)
        {
            entities[i].Id = savedDbObjects[i].Id;
        }

        return [.. savedDbObjects.Select(item => item.Id)];
    }

    public virtual Task DeleteAsync(TId id)
    {
        var filter = BuildDbNotDeletedFilter(id);
        return SoftDeleteAsync(filter, (f, u) => Collection.UpdateOneAsync(f, u));
    }

    public virtual Task DeleteAsync(TSearchParams searchParams)
    {
        ArgumentNullException.ThrowIfNull(searchParams);
        var filter = BuildDbFilter(searchParams);
        return SoftDeleteAsync(filter, (f, u) => Collection.UpdateManyAsync(f, u));
    }

    private static FilterDefinition<TDbModel> BuildDbNotDeletedFilter(TId id)
    {
        return Builders<TDbModel>.Filter.And(
            Builders<TDbModel>.Filter.Eq(item => item.Id, id),
            Builders<TDbModel>.Filter.Eq(item => item.DeletedAt, null));
    }

    private static FilterDefinition<TDbModel> BuildDbFilter(TSearchParams searchParams)
    {
        var builder = Builders<TDbModel>.Filter;
        var filter = builder.Empty;

        if (searchParams.IsDeleted)
        {
            filter = builder.And(filter, builder.Ne(item => item.DeletedAt, null));
        }
        else
        {
            filter = builder.And(filter, builder.Eq(item => item.DeletedAt, null));
        }

        if (searchParams.CreatedFrom.HasValue)
        {
            filter = builder.And(filter, builder.Gte(item => item.CreatedAt, searchParams.CreatedFrom.Value));
        }
        if (searchParams.CreatedTo.HasValue)
        {
            filter = builder.And(filter, builder.Lte(item => item.CreatedAt, searchParams.CreatedTo.Value));
        }
        if (searchParams.UpdatedFrom.HasValue)
        {
            filter = builder.And(filter, builder.Gte(item => item.UpdatedAt, searchParams.UpdatedFrom.Value));
        }
        if (searchParams.UpdatedTo.HasValue)
        {
            filter = builder.And(filter, builder.Lte(item => item.UpdatedAt, searchParams.UpdatedTo.Value));
        }
        if (searchParams.DeletedFrom.HasValue)
        {
            filter = builder.And(filter, builder.Gte(item => item.DeletedAt, searchParams.DeletedFrom.Value));
        }
        if (searchParams.DeletedTo.HasValue)
        {
            filter = builder.And(filter, builder.Lte(item => item.DeletedAt, searchParams.DeletedTo.Value));
        }

        filter = TFilter.Filter(filter, builder, searchParams);

        return filter;
    }

    private static SortDefinition<TDbModel> BuildDbSort(TSearchParams searchParams)
    {
        var builder = Builders<TDbModel>.Sort;

        var sort = builder
            .Descending(item => item.UpdatedAt)
            .Descending(item => item.CreatedAt)
            .Descending(item => item.Id);

        if (!string.IsNullOrWhiteSpace(searchParams.SortField))
        {
            sort = searchParams.SortOrder == SortOrder.Descending
                ? builder.Descending(searchParams.SortField)
                : builder.Ascending(searchParams.SortField);
        }

        return sort;
    }

    private static IFindFluent<TDbModel, TDbModel> BuildDbPagination(IFindFluent<TDbModel, TDbModel> find, TSearchParams searchParams)
    {
        if (searchParams.ObjectsCount == 0)
        {
            return find.Limit(0);
        }

        find = find.Skip((searchParams.Page - 1) * searchParams.ObjectsCount ?? 0);

        if (searchParams.ObjectsCount.HasValue)
        {
            find = find.Limit(searchParams.ObjectsCount.Value);
        }

        return find;
    }

    [SuppressMessage("CodeQuality", "IDE0060:Remove unused parameter", Justification = "Параметр TConvertParams оставлен для совместимости с общим контрактом.")]
    private static async Task<IList<TEntity>> BuildEntitiesListAsync(IFindFluent<TDbModel, TDbModel> dbObjects, TConvertParams convertParams)
    {
        var entities = (await dbObjects.ToListAsync()).Select(TMapper.ToEntity).ToList();
        return entities;
    }

    private static TDbModel AddOrUpdate(TEntity entity, TDbModel? dbObject, bool exists, DateTime now)
    {
        dbObject ??= new TDbModel();

        TMapper.ToDbModel(entity, dbObject);

        if (!exists)
        {
            dbObject.CreatedAt = now;
        }
        dbObject.UpdatedAt = now;

        return dbObject;
    }

    private static Task<UpdateResult> SoftDeleteAsync(FilterDefinition<TDbModel> filter, Func<FilterDefinition<TDbModel>, UpdateDefinition<TDbModel>, Task<UpdateResult>> execute)
    {
        var now = DateTime.UtcNow;

        var update = Builders<TDbModel>.Update
            .Set(item => item.DeletedAt, now)
            .Set(item => item.UpdatedAt, now);

        return execute(filter, update);
    }
}
