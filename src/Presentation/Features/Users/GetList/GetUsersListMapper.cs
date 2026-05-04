using Application.Abstractions.Persistence.Read.Paged;
using Application.Users.GetList;

using Riok.Mapperly.Abstractions;

namespace Presentation.Features.Users.GetList;

[Mapper(IncludedConstructors = MemberVisibility.All)]
internal static partial class GetUsersListMapper
{
    internal static partial UsersFilterParameters ToFilterParameters(GetUsersListRequest request);

    internal static partial PagedResult<UserListItemResponse> ToResponse(
        PagedResult<UserListItemReadModel> pagedResult);
}
