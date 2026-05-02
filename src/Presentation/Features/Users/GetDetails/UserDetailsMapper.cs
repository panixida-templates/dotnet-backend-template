using Application.Users.GetDetails;

using Riok.Mapperly.Abstractions;

namespace Presentation.Features.Users.GetDetails;

[Mapper]
internal static partial class UserDetailsMapper
{
    internal static partial UserDetailsResponse ToResponse(UserDetailsReadModel dto);
}
