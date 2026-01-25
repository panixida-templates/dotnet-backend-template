using Entities;

using Pl.Api.Http.Dtos.Models;
using Pl.Api.Http.Mappers.Core;

using Riok.Mapperly.Abstractions;

namespace Pl.Api.Http.Mappers;

[Mapper]
public sealed partial class UsersMapper : IMapper<UserDto, User>
{
    public static partial UserDto ToDto(User entity);
    public static partial IEnumerable<UserDto> ToDto(IEnumerable<User> entities);
    public static partial User ToEntity(UserDto dto);
    public static partial IEnumerable<User> ToEntity(IEnumerable<UserDto> dtos);
}
