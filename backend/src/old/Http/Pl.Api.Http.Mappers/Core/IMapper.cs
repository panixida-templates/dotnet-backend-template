namespace Pl.Api.Http.Mappers.Core;

public interface IMapper<TDto, TEntity>
{
    static abstract TDto ToDto(TEntity entity);
    static abstract IEnumerable<TDto> ToDto(IEnumerable<TEntity> entities);
    static abstract TEntity ToEntity(TDto dto);
    static abstract IEnumerable<TEntity> ToEntity(IEnumerable<TDto> dtos);
}
