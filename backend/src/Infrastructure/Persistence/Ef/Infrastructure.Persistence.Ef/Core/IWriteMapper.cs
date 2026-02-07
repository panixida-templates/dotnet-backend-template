using Domain.Entities.Core;

namespace Infrastructure.Persistence.Ef.Core;

internal interface IWriteMapper<TId, TDbModel, TEntity>
    where TId : struct
    where TDbModel : DbModel<TId>
    where TEntity : Entity<TId>
{
    static abstract void ToDbModel(TEntity entity, TDbModel dbModel);
    static abstract TEntity ToEntity(TDbModel dbModel);
}
