using Application.Users.Get;

using Riok.Mapperly.Abstractions;

namespace Presentation.Http.Features.Users.GetById;

[Mapper]
internal static partial class GetByIdUserMapper
{
    internal static partial GetByIdUserResponse ToResponse(GetByIdUserDto dto);
}
