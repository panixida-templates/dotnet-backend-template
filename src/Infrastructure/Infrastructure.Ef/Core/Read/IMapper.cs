namespace Infrastructure.Ef.Core.Read;

internal interface IMapper<TId, TDbModel, TDto>
    where TId : struct
    where TDbModel : ReadDbModel<TId>
{
    static abstract IQueryable<TDto> ProjectTo(IQueryable<TDbModel> query);
}
