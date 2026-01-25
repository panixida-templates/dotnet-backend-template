using Dal.Ef.DbModels.Core;

namespace Dal.Ef.Includes.Core;

public interface IInclude<TId, TDbModel, TConvertParams>
    where TId : struct
    where TDbModel : BaseDbModel<TId>
    where TConvertParams : class
{
    static abstract IQueryable<TDbModel> Include(IQueryable<TDbModel> dbObjects, TConvertParams convertParams);
}
