using Application.Abstractions.Queries;

using Microsoft.EntityFrameworkCore;

using System.Linq.Dynamic.Core;

namespace Infrastructure.Persistence.Ef.Core;

internal abstract class QueryService
    <TDbContext, TId, TDbModel, TGetByIdDto, TSearchDto, TSearchParams, TGetByIdMapper, TSearchMapper, TFilter>(TDbContext dbContext)
    : IQueryService<TId, TGetByIdDto, TSearchDto, TSearchParams>
    where TDbContext : DbContext
    where TId : struct
    where TDbModel : DbModel<TId>
    where TGetByIdDto : class
    where TSearchDto : class
    where TSearchParams : BaseSearchParams
    where TGetByIdMapper : IDtoMapper<TId, TDbModel, TGetByIdDto>
    where TSearchMapper : IDtoMapper<TId, TDbModel, TSearchDto>
    where TFilter : IFilter<TId, TDbModel, TSearchParams>
{
    private static readonly bool IsAuditable = typeof(AuditableDbModel<TId>).IsAssignableFrom(typeof(TDbModel));

    public virtual async Task<TGetByIdDto?> GetByIdAsync(TId id, CancellationToken cancellationToken)
    {
        var dbObjects = dbContext.Set<TDbModel>().AsNoTracking();
        dbObjects = QueryableExtensions<TId, TDbModel>.ApplyGetByIdFilter(dbObjects, id);

        var dtoQuery = TGetByIdMapper.ProjectTo(dbObjects);

        var dto = await dtoQuery.FirstOrDefaultAsync(cancellationToken);
        if (dto is null)
        {
            return null;
        }

        return dto;
    }

    public virtual async Task<SearchResult<TSearchDto>> SearchAsync(TSearchParams searchParams, CancellationToken cancellationToken)
    {
        IQueryable<TDbModel> dbObjects = dbContext.Set<TDbModel>().AsNoTracking();
        dbObjects = BuildDbFilter(dbObjects, searchParams);
        dbObjects = BuildDbSort(dbObjects, searchParams);

        var total = await dbObjects.CountAsync(cancellationToken);

        dbObjects = BuildDbPagination(dbObjects, searchParams);

        var dtoQuery = TSearchMapper.ProjectTo(dbObjects);
        var dtos = await dtoQuery.ToListAsync(cancellationToken);

        return new SearchResult<TSearchDto>
        {
            Total = total,
            Objects = dtos,
            RequestedObjectsCount = searchParams.ObjectsCount,
            RequestedPage = searchParams.Page,
        };
    }

    public virtual Task<bool> ExistsByIdAsync(TId id, CancellationToken cancellationToken)
    {
        var dbObjects = dbContext.Set<TDbModel>().AsNoTracking();
        dbObjects = QueryableExtensions<TId, TDbModel>.ApplyGetByIdFilter(dbObjects, id);
        return dbObjects.AnyAsync(cancellationToken);
    }

    public virtual Task<bool> AnyAsync(TSearchParams searchParams, CancellationToken cancellationToken)
    {
        var data = dbContext;
        var dbObjects = data.Set<TDbModel>().AsNoTracking();
        return BuildDbFilter(dbObjects, searchParams).AnyAsync(cancellationToken);
    }

    private static IQueryable<TDbModel> BuildDbFilter(IQueryable<TDbModel> dbObjects, TSearchParams searchParams)
    {
        if (IsAuditable)
        {
            dbObjects = ApplyAuditableFilter(dbObjects, searchParams);
        }

        dbObjects = TFilter.Filter(dbObjects, searchParams);

        return dbObjects;
    }

    private static IQueryable<TDbModel> ApplyAuditableFilter(IQueryable<TDbModel> dbObjects, TSearchParams searchParams)
    {
        if (searchParams.IsDeleted)
        {
            dbObjects = dbObjects.Where(item =>
                EF.Property<DateTime?>(item, nameof(AuditableDbModel<>.DeletedAt)).HasValue);
        }
        else
        {
            dbObjects = dbObjects.Where(item =>
                !EF.Property<DateTime?>(item, nameof(AuditableDbModel<>.DeletedAt)).HasValue);
        }

        if (searchParams.CreatedFrom.HasValue)
        {
            dbObjects = dbObjects.Where(item =>
                searchParams.CreatedFrom.Value <= EF.Property<DateTime>(item, nameof(AuditableDbModel<>.CreatedAt)));
        }

        if (searchParams.CreatedTo.HasValue)
        {
            dbObjects = dbObjects.Where(item =>
                EF.Property<DateTime>(item, nameof(AuditableDbModel<>.CreatedAt)) <= searchParams.CreatedTo.Value);
        }

        if (searchParams.UpdatedFrom.HasValue)
        {
            dbObjects = dbObjects.Where(item =>
                searchParams.UpdatedFrom.Value <= EF.Property<DateTime>(item, nameof(AuditableDbModel<>.UpdatedAt)));
        }

        if (searchParams.UpdatedTo.HasValue)
        {
            dbObjects = dbObjects.Where(item =>
                EF.Property<DateTime>(item, nameof(AuditableDbModel<>.UpdatedAt)) <= searchParams.UpdatedTo.Value);
        }

        if (searchParams.DeletedFrom.HasValue)
        {
            dbObjects = dbObjects.Where(item =>
                searchParams.DeletedFrom.Value <= EF.Property<DateTime?>(item, nameof(AuditableDbModel<>.DeletedAt)));
        }

        if (searchParams.DeletedTo.HasValue)
        {
            dbObjects = dbObjects.Where(item =>
                EF.Property<DateTime?>(item, nameof(AuditableDbModel<>.DeletedAt)) <= searchParams.DeletedTo.Value);
        }

        return dbObjects;
    }

    private static IQueryable<TDbModel> BuildDbSort(IQueryable<TDbModel> dbObjects, TSearchParams searchParams)
    {
        if (!string.IsNullOrWhiteSpace(searchParams.SortField))
        {
            if (searchParams.SortOrder == SortOrder.Descending)
            {
                return dbObjects.OrderBy($"{searchParams.SortField} descending");
            }

            return dbObjects.OrderBy(searchParams.SortField);
        }

        var visitor = new OrderedQueryableVisitor();
        visitor.Visit(dbObjects.Expression);

        var isOrdered = visitor.IsOrdered;

        if (isOrdered && dbObjects is IOrderedQueryable<TDbModel> orderedObjects)
        {
            if (IsAuditable)
            {
                return orderedObjects
                    .ThenByDescending(item => EF.Property<DateTime>(item, nameof(AuditableDbModel<>.UpdatedAt)))
                    .ThenByDescending(item => EF.Property<DateTime>(item, nameof(AuditableDbModel<>.CreatedAt)))
                    .ThenByDescending(item => item.Id);
            }

            return orderedObjects
                .ThenByDescending(item => item.Id);
        }

        if (IsAuditable)
        {
            return dbObjects
                .OrderByDescending(item => EF.Property<DateTime>(item, nameof(AuditableDbModel<>.UpdatedAt)))
                .ThenByDescending(item => EF.Property<DateTime>(item, nameof(AuditableDbModel<>.CreatedAt)))
                .ThenByDescending(item => item.Id);
        }

        return dbObjects
            .OrderByDescending(item => item.Id);
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
}
