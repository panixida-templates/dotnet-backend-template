using Application.Users.Search;

using Infrastructure.Persistence.Ef.Core;

using Riok.Mapperly.Abstractions;

namespace Infrastructure.Persistence.Ef.Features.Users;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal sealed partial class SearchUserDtoMapper : IDtoMapper<Guid, UserDbModel, SearchUserDto>
{
    public static partial IQueryable<SearchUserDto> ProjectTo(IQueryable<UserDbModel> query);
}
