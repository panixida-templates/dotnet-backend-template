using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Ef.Core;

internal static class QueryableExtensions<TId, TDbModel>
    where TId : struct
    where TDbModel : DbModel<TId>
{
    private static readonly bool IsAuditable = typeof(AuditableDbModel<TId>).IsAssignableFrom(typeof(TDbModel));

    internal static IQueryable<TDbModel> ApplyGetByIdFilter(IQueryable<TDbModel> dbObjects, TId id)
    {
        if (IsAuditable)
        {
            dbObjects = dbObjects.Where(item =>
                item.Id.Equals(id) &&
                !EF.Property<DateTime?>(item, nameof(AuditableDbModel<>.DeletedAt)).HasValue);
        }
        else
        {
            dbObjects = dbObjects.Where(item => item.Id.Equals(id));
        }

        return dbObjects;
    }
}
