using Entities;

using Pl.Api.Http.Dtos.Models;

using Riok.Mapperly.Abstractions;

namespace Pl.Api.Http.Mappers;

[Mapper]
public static partial class UsersMapper
{
    public static partial User ToEntity(this UserDto dto);
    public static partial List<User> ToEntity(this IEnumerable<UserDto> dtos);
    public static partial UserDto ToDto(this User entity);
    public static partial List<UserDto> ToDto(this IEnumerable<User> entities);
}
