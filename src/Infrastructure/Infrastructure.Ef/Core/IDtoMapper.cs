namespace Infrastructure.Persistence.Ef.Core;

internal interface IDtoMapper<TId, TDbModel, TDto>
    where TId : struct
    where TDbModel : DbModel<TId>
    where TDto : class
{
    static abstract IQueryable<TDto> ProjectTo(IQueryable<TDbModel> query);
}
