using Organization.Product.Module.Application.RepositoryExamples.Users.GetDetails;

using Riok.Mapperly.Abstractions;

namespace Organization.Product.Module.Presentation.RepositoryExamples.Features.Users.GetDetails;

[Mapper]
internal static partial class UserDetailsMapper
{
    internal static partial UserDetailsResponse ToResponse(UserDetailsReadModel dto);
}
