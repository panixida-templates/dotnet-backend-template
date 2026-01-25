using Dal.Ef.DbModels.Core;

using Entities.Core;

namespace Dal.Ef.Mappers.Core;

public interface IMapper<TId, TDbModel, TEntity>
    where TId : struct
    where TDbModel : BaseDbModel<TId>
    where TEntity : BaseEntity<TId>
{
    static abstract void ToDbModel(TEntity entity, TDbModel dbModel);
    static abstract TEntity ToEntity(TDbModel dbModel);
}
