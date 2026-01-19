using Dal.MongoDb.DbModels.Core;

using Entities.Core;

namespace Dal.MongoDb.Mappers.Core;

public interface IMapper<TId, TDbModel, TEntity>
    where TId : struct
    where TDbModel : BaseDbModel<TId>
    where TEntity : BaseEntity<TId>
{
    static abstract void ToDbModel(TEntity entity, TDbModel dbModel);
    static abstract TEntity ToEntity(TDbModel dbModel);
}
