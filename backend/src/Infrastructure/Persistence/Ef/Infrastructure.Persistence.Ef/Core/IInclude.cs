namespace Infrastructure.Persistence.Ef.Core;

internal interface IInclude<TId, TDbModel, TConvertParams>
    where TId : struct
    where TDbModel : DbModel<TId>
    where TConvertParams : class
{
    static abstract IQueryable<TDbModel> Include(IQueryable<TDbModel> dbObjects, TConvertParams convertParams);
}
