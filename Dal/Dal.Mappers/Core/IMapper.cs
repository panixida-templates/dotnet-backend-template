namespace Dal.Mappers.Core;

public interface IMapper<TDbModel, TEntity>
{
    static abstract TDbModel ToDbModel(TEntity entity);
    static abstract IEnumerable<TDbModel> ToDbModel(IEnumerable<TEntity> entities);
    static abstract TEntity ToEntity(TDbModel dbModel);
    static abstract IEnumerable<TEntity> ToEntity(IEnumerable<TDbModel> dbModels);
}
