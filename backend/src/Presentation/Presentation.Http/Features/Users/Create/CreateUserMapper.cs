using Application.Users.Create;

using Riok.Mapperly.Abstractions;

namespace Presentation.Http.Features.Users.Create;

[Mapper]
internal static partial class CreateUserMapper
{
    internal static partial CreateUserCommand ToCommand(CreateUserRequest request, Guid id);
    internal static partial CreateUserResponse ToResponse(Guid id);
}
