using Application.Abstractions.Persistence.Core;

using Common.Enums;
using Common.Exceptions;
using Common.SearchParams.Core;

using Microsoft.EntityFrameworkCore;

using System.Linq.Dynamic.Core;

namespace Infrastructure.Persistence.Ef.Core;

internal abstract class EfReadRepository<TDbContext, TId, TDbModel, TDto, TSearchParams, TMapper, TFilter>(TDbContext dbContext)
    : IReadRepository<TId, TDto, TSearchParams>
    where TDbContext : DbContext
    where TId : struct
    where TDbModel : DbModel<TId>
    where TDto : class
    where TSearchParams : BaseSearchParams
    where TMapper : IReadMapper<TId, TDbModel, TDto>
    where TFilter : IFilter<TId, TDbModel, TSearchParams>
{
    public virtual async Task<TDto> GetAsync(TId id, CancellationToken cancellationToken)
    {
        var dbObjects = dbContext.Set<TDbModel>().AsNoTracking();
        dbObjects = EfFilters<TId, TDbModel>.BuildDbNotDeletedFilter(dbObjects, id);

        var dtoQuery = TMapper.ProjectTo(dbObjects);

        var dto = await dtoQuery.FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"{typeof(TDbModel).Name} с id={id} не найдена");

        return dto;
    }

    public virtual async Task<SearchResult<TDto>> GetAsync(TSearchParams searchParams, CancellationToken cancellationToken)
    {
        IQueryable<TDbModel> dbObjects = dbContext.Set<TDbModel>().AsNoTracking();
        dbObjects = EfFilters<TId, TDbModel>.BuildDbFilter<TSearchParams, TFilter>(dbObjects, searchParams);
        dbObjects = BuildDbSort(dbObjects, searchParams);

        var total = await dbObjects.CountAsync(cancellationToken);

        dbObjects = BuildDbPagination(dbObjects, searchParams);

        var dtoQuery = TMapper.ProjectTo(dbObjects);
        var dtos = await dtoQuery.ToListAsync(cancellationToken);

        return new SearchResult<TDto>
        {
            Total = total,
            Objects = dtos,
            RequestedObjectsCount = searchParams.ObjectsCount,
            RequestedPage = searchParams.Page,
        };
    }

    public virtual Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken)
    {
        var dbObjects = dbContext.Set<TDbModel>().AsNoTracking();
        dbObjects = EfFilters<TId, TDbModel>.BuildDbNotDeletedFilter(dbObjects, id);
        return dbObjects.AnyAsync(cancellationToken);
    }

    public virtual Task<bool> ExistsAsync(TSearchParams searchParams, CancellationToken cancellationToken)
    {
        var data = dbContext;
        var dbObjects = data.Set<TDbModel>().AsNoTracking();
        return EfFilters<TId, TDbModel>.BuildDbFilter<TSearchParams, TFilter>(dbObjects, searchParams).AnyAsync(cancellationToken);
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
}
