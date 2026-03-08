using Application.Users.Update;

using Riok.Mapperly.Abstractions;

namespace Presentation.Http.Features.Users.Update;

[Mapper]
internal static partial class UpdateUserMapper
{
    internal static partial UpdateUserCommand ToCommand(UpdateUserRequest request, Guid id);
}
