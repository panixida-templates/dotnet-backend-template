using Pl.Api.Http.Mappers.Core;

using Presentation.Api.Http.Dtos.Models;

using Riok.Mapperly.Abstractions;

using ApplicationUserDto = Application.Contracts.Dtos.Users.UserDto;

namespace Presentation.Api.Http.Mappers;

[Mapper]
public sealed partial class UsersMapper : IMapper<UserDto, ApplicationUserDto>
{
    public static partial UserDto ToDto(ApplicationUserDto entity);
    public static partial IEnumerable<UserDto> ToDto(IEnumerable<ApplicationUserDto> entities);

    public static partial ApplicationUserDto ToEntity(UserDto dto);
    public static partial IEnumerable<ApplicationUserDto> ToEntity(IEnumerable<UserDto> dtos);
}
