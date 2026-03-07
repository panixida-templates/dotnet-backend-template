using Domain.Abstractions;

namespace Infrastructure.Persistence.Ef.Core;

internal interface IEntityMapper<TId, TDbModel, TEntity>
    where TId : struct
    where TDbModel : DbModel<TId>
    where TEntity : Entity<TId>
{
    static abstract void ToDbModel(TEntity entity, TDbModel dbModel);
    static abstract TEntity ToEntity(TDbModel dbModel);
}
