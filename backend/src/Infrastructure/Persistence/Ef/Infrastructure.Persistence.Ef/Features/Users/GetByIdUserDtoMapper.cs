using Application.Users.Get;

using Infrastructure.Persistence.Ef.Core;

using Riok.Mapperly.Abstractions;

namespace Infrastructure.Persistence.Ef.Features.Users;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal sealed partial class GetByIdUserDtoMapper : IDtoMapper<Guid, UserDbModel, GetByIdUserDto>
{
    public static partial IQueryable<GetByIdUserDto> ProjectTo(IQueryable<UserDbModel> query);
}