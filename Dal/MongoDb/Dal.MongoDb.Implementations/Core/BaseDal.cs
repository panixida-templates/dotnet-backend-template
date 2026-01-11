using Common.Enums;
using Common.Exceptions;
using Common.SearchParams.Core;

using Dal.Interfaces.Core;
using Dal.MongoDb.DbModels.Core;
using Dal.MongoDb.Mappers.Core;

using Entities.Core;

using MongoDB.Bson;
using MongoDB.Driver;
using System.Runtime.CompilerServices;

namespace Dal.MongoDb.Implementations.Core;

public abstract class BaseDal<TId, TDbModel, TEntity, TMapper, TSearchParams, TConvertParams>
    : IBaseDal<TId, TEntity, TSearchParams, TConvertParams>
    where TId : struct
    where TDbModel : BaseDbModel<TId>, new()
    where TEntity : BaseEntity<TId>
    where TMapper : IMapper<TDbModel, TEntity>
    where TSearchParams : BaseSearchParams
    where TConvertParams : class, new()
{
    protected IMongoCollection<TDbModel> Collection { get; }

    protected abstract string CollectionName { get; }

    protected BaseDal(IMongoDatabase database)
    {
        Collection = database.GetCollection<TDbModel>(CollectionName);
    }

    public virtual async Task<TEntity> GetAsync(TId id, TConvertParams? convertParams = null)
    {
        convertParams ??= new TConvertParams();
        var filter = BuildNotDeletedFilter(id);

        var dbObject = await Collection.Find(filter).Limit(1).FirstOrDefaultAsync() //TODO поверить на n+1
            ?? throw new NotFoundException($"{typeof(TDbModel).Name} с id={id} не найдена");

        var entity = (await BuildEntitiesListAsync([dbObject], convertParams))[0];

        return entity;
    }

    public virtual async Task<SearchResult<TEntity>> GetAsync(TSearchParams searchParams, TConvertParams? convertParams = null)
    {
        ArgumentNullException.ThrowIfNull(searchParams);
        convertParams ??= new TConvertParams();

        var filter = await BuildDbFilterAsync(searchParams);
        var sort = await BuildSortAsync(searchParams);

        var searchResult = new SearchResult<TEntity>
        {
            Total = await Collection.CountDocumentsAsync(filter),
            Objects = [],
            RequestedObjectsCount = searchParams.ObjectsCount,
            RequestedPage = searchParams.Page,
        };

        if (searchParams.ObjectsCount == 0)
        {
            return searchResult;
        }

        var find = Collection.Find(filter).Sort(sort);

        var skip = (searchParams.Page - 1) * (searchParams.ObjectsCount ?? 0);
        if (skip > 0)
        {
            find = find.Skip(skip);
        }

        if (searchParams.ObjectsCount.HasValue)
        {
            find = find.Limit(searchParams.ObjectsCount.Value);
        }

        var dbObjects = await find.ToListAsync();
        searchResult.Objects = await BuildEntitiesListAsync(dbObjects, convertParams); // TODO: N+1

        return searchResult;
    }

    public virtual async Task<bool> ExistsAsync(TId id)
    {
        var filter = BuildNotDeletedFilter(id);
        return await Collection.Find(filter).AnyAsync();
    }

    public virtual async Task<bool> ExistsAsync(TSearchParams searchParams)
    {
        ArgumentNullException.ThrowIfNull(searchParams);
        var filter = await BuildDbFilterAsync(searchParams);
        return await Collection.Find(filter).AnyAsync();
    }

    public virtual async Task<TId> AddOrUpdateAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var id = EnsureId(entity.Id);
        entity.Id = id;

        var filter = Builders<TDbModel>.Filter.Eq(item => item.Id, id);
        var existing = await Collection.Find(filter).Limit(1).FirstOrDefaultAsync();

        var dbObject = TMapper.ToDbModel(entity);

        var now = DateTime.UtcNow;

        if (existing is null)
        {
            dbObject.CreatedAt = now;
            dbObject.DeletedAt = null;
        }
        else
        {
            dbObject.CreatedAt = existing.CreatedAt;
            dbObject.DeletedAt = existing.DeletedAt;
        }

        dbObject.UpdatedAt = now;

        await Collection.ReplaceOneAsync(filter, dbObject, new ReplaceOptions { IsUpsert = true });

        entity.Id = dbObject.Id;
        return dbObject.Id;
    }

    public virtual async Task<IList<TId>> AddOrUpdateAsync(IList<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (entities.Count == 0)
        {
            return [];
        }

        for (var i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];
            var id = EnsureId(entity.Id);
            entity.Id = id;
        }

        var ids = entities.Select(item => item.Id).ToArray();

        var existingFilter = Builders<TDbModel>.Filter.In(item => item.Id, ids);
        var existingDocs = await Collection.Find(existingFilter).ToListAsync();
        var existingDict = existingDocs.ToDictionary(item => item.Id);

        var now = DateTime.UtcNow;

        var models = new List<WriteModel<TDbModel>>(entities.Count);
        var resultIds = new List<TId>(entities.Count);

        foreach (var entity in entities)
        {
            var dbObject = TMapper.ToDbModel(entity);

            if (existingDict.TryGetValue(entity.Id, out var existing))
            {
                dbObject.CreatedAt = existing.CreatedAt;
                dbObject.DeletedAt = existing.DeletedAt;
            }
            else
            {
                dbObject.CreatedAt = now;
                dbObject.DeletedAt = null;
            }

            dbObject.UpdatedAt = now;

            var filter = Builders<TDbModel>.Filter.Eq(x => x.Id, entity.Id);

            models.Add(new ReplaceOneModel<TDbModel>(filter, dbObject) { IsUpsert = true });
            resultIds.Add(dbObject.Id);
        }

        await Collection.BulkWriteAsync(models, new BulkWriteOptions { IsOrdered = false });

        return resultIds;
    }

    public virtual Task DeleteAsync(TId id)
    {
        var filter = BuildNotDeletedFilter(id);
        return SoftDeleteAsync(filter, (f, u) => Collection.UpdateOneAsync(f, u));
    }

    public virtual async Task DeleteAsync(TSearchParams searchParams)
    {
        ArgumentNullException.ThrowIfNull(searchParams);
        var filter = await BuildDbFilterAsync(searchParams);
        await SoftDeleteAsync(filter, (f, u) => Collection.UpdateManyAsync(f, u));
    }

    protected virtual ValueTask<FilterDefinition<TDbModel>> BuildDbFilterAsync(TSearchParams searchParams)
    {
        var filter = Builders<TDbModel>.Filter.Empty;

        if (searchParams.IsDeleted)
        {
            filter = Builders<TDbModel>.Filter.And(filter, Builders<TDbModel>.Filter.Ne(item => item.DeletedAt, null));
        }
        else
        {
            filter = Builders<TDbModel>.Filter.And(filter, Builders<TDbModel>.Filter.Eq(item => item.DeletedAt, null));
        }

        if (searchParams.CreatedFrom.HasValue)
        {
            filter = Builders<TDbModel>.Filter.And(filter, Builders<TDbModel>.Filter.Gte(item => item.CreatedAt, searchParams.CreatedFrom.Value));
        }
        if (searchParams.CreatedTo.HasValue)
        {
            filter = Builders<TDbModel>.Filter.And(filter, Builders<TDbModel>.Filter.Lte(item => item.CreatedAt, searchParams.CreatedTo.Value));
        }
        if (searchParams.UpdatedFrom.HasValue)
        {
            filter = Builders<TDbModel>.Filter.And(filter, Builders<TDbModel>.Filter.Gte(item => item.UpdatedAt, searchParams.UpdatedFrom.Value));
        }
        if (searchParams.UpdatedTo.HasValue)
        {
            filter = Builders<TDbModel>.Filter.And(filter, Builders<TDbModel>.Filter.Lte(item => item.UpdatedAt, searchParams.UpdatedTo.Value));
        }
        if (searchParams.DeletedFrom.HasValue)
        {
            filter = Builders<TDbModel>.Filter.And(filter, Builders<TDbModel>.Filter.Gte(item => item.DeletedAt, searchParams.DeletedFrom.Value));
        }
        if (searchParams.DeletedTo.HasValue)
        {
            filter = Builders<TDbModel>.Filter.And(filter, Builders<TDbModel>.Filter.Lte(item => item.DeletedAt, searchParams.DeletedTo.Value));
        }

        return ValueTask.FromResult(filter);
    }

    protected virtual ValueTask<SortDefinition<TDbModel>> BuildSortAsync(TSearchParams searchParams)
    {
        SortDefinition<TDbModel> sort;

        if (!string.IsNullOrWhiteSpace(searchParams.SortField))
        {
            if (searchParams.SortOrder == SortOrder.Descending)
            {
                sort = Builders<TDbModel>.Sort.Descending(searchParams.SortField);
            }
            else
            {
                sort = Builders<TDbModel>.Sort.Ascending(searchParams.SortField);
            }
        }
        else
        {
            sort = Builders<TDbModel>.Sort
                .Descending(item => item.UpdatedAt)
                .Descending(item => item.CreatedAt)
                .Descending(item => item.Id);
        }

        return ValueTask.FromResult(sort);
    }

    protected abstract Task<IList<TEntity>> BuildEntitiesListAsync(IReadOnlyList<TDbModel> dbObjects, TConvertParams convertParams);

    private static FilterDefinition<TDbModel> BuildNotDeletedFilter(TId id)
    {
        return Builders<TDbModel>.Filter.And(
            Builders<TDbModel>.Filter.Eq(item => item.Id, id),
            Builders<TDbModel>.Filter.Eq(item => item.DeletedAt, null));
    }

    private static TId EnsureId(TId id)
    {
        if (!EqualityComparer<TId>.Default.Equals(id, default))
        {
            return id;
        }

        if (typeof(TId) == typeof(ObjectId))
        {
            var newId = ObjectId.GenerateNewId();
            return Unsafe.As<ObjectId, TId>(ref newId);
        }
        if (typeof(TId) == typeof(Guid))
        {
            var newId = Guid.NewGuid();
            return Unsafe.As<Guid, TId>(ref newId);
        }

        throw new NotImplementedException(
            $"Генерация Id для типа {typeof(TId).Name} не реализована. " +
            "Либо задайте Id заранее, либо добавьте генерацию для этого типа.");
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
