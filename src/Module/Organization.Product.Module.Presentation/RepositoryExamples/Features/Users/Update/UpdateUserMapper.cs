using Organization.Product.Module.Application.RepositoryExamples.Users.Update;

using Riok.Mapperly.Abstractions;

namespace Organization.Product.Module.Presentation.RepositoryExamples.Features.Users.Update;

[Mapper]
internal static partial class UpdateUserMapper
{
    internal static partial UpdateUserCommand ToCommand(UpdateUserRequest request, Guid id);
}
