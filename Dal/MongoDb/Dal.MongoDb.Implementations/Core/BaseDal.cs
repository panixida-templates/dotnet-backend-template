using Common.Enums;
using Common.Exceptions;
using Common.SearchParams.Core;

using Dal.Interfaces.Core;

using Dal.MongoDb.DbModels.Core;
using Dal.MongoDb.Filters.Core;
using Dal.MongoDb.Mappers.Core;

using Entities.Core;

using MongoDB.Bson;
using MongoDB.Driver;

using System.Runtime.CompilerServices;

namespace Dal.MongoDb.Implementations.Core;

public abstract class BaseDal<TId, TDbModel, TEntity, TSearchParams, TConvertParams, TMapper, TFilter>(IMongoDatabase database, string collectionName)
    : IBaseDal<TId, TEntity, TSearchParams, TConvertParams>
    where TId : struct
    where TDbModel : BaseDbModel<TId>, new()
    where TEntity : BaseEntity<TId>
    where TSearchParams : BaseSearchParams
    where TConvertParams : class, new()
    where TMapper : IMapper<TDbModel, TEntity>
    where TFilter : IFilter<TId, TDbModel, TSearchParams>
{
    protected IMongoCollection<TDbModel> Collection { get; } = database.GetCollection<TDbModel>(collectionName);

    public virtual async Task<TEntity> GetAsync(TId id, TConvertParams? convertParams = null)
    {
        convertParams ??= new TConvertParams();
        var filter = BuildDbNotDeletedFilter(id);

        var dbObject = await Collection.Find(filter).Limit(1).FirstOrDefaultAsync()
            ?? throw new NotFoundException($"{typeof(TDbModel).Name} с id={id} не найдена");

        var entity = (await BuildEntitiesListAsync([dbObject], convertParams))[0];

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

        var find = Collection.Find(filter).Sort(sort);
        find = BuildDbPagination(find, searchParams);

        var dbObjects = await find.ToListAsync();
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

        var now = DateTime.UtcNow;

        if (EqualityComparer<TId>.Default.Equals(entity.Id, default))
        {
            var newDbObject = TMapper.ToDbModel(entity);

            newDbObject.CreatedAt = now;
            newDbObject.UpdatedAt = now;
            newDbObject.DeletedAt = null;

            await Collection.InsertOneAsync(newDbObject);

            entity.Id = newDbObject.Id;
            return newDbObject.Id;
        }

        // UPDATE/UPSERT с уже заданным Id
        var id = entity.Id;
        var filter = Builders<TDbModel>.Filter.Eq(item => item.Id, id);

        var existing = await Collection.Find(filter).Limit(1).FirstOrDefaultAsync();

        var dbObject = TMapper.ToDbModel(entity);

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
            //var id = EnsureId(entity.Id);
            entity.Id = entity.Id;
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

    protected abstract Task<IList<TEntity>> BuildEntitiesListAsync(IReadOnlyList<TDbModel> dbObjects, TConvertParams convertParams);

    //private static TId EnsureId(TId id)
    //{
    //    if (!EqualityComparer<TId>.Default.Equals(id, default))
    //    {
    //        return id;
    //    }

    //    if (typeof(TId) == typeof(ObjectId))
    //    {
    //        var newId = ObjectId.GenerateNewId();
    //        return Unsafe.As<ObjectId, TId>(ref newId);
    //    }
    //    if (typeof(TId) == typeof(Guid))
    //    {
    //        var newId = Guid.NewGuid();
    //        return Unsafe.As<Guid, TId>(ref newId);
    //    }

    //    throw new NotImplementedException(
    //        $"Генерация Id для типа {typeof(TId).Name} не реализована. " +
    //        "Либо задайте Id заранее, либо добавьте генерацию для этого типа.");
    //}

    private static Task<UpdateResult> SoftDeleteAsync(FilterDefinition<TDbModel> filter, Func<FilterDefinition<TDbModel>, UpdateDefinition<TDbModel>, Task<UpdateResult>> execute)
    {
        var now = DateTime.UtcNow;

        var update = Builders<TDbModel>.Update
            .Set(item => item.DeletedAt, now)
            .Set(item => item.UpdatedAt, now);

        return execute(filter, update);
    }
}
