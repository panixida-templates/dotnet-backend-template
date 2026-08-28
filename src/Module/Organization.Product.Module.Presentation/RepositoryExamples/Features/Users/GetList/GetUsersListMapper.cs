using Organization.Product.Module.Application.Users;
using Organization.Product.Module.Application.Users.GetList;

using Riok.Mapperly.Abstractions;

namespace Organization.Product.Module.Presentation.Features.Users.GetList;

[Mapper]
internal static partial class GetUsersListMapper
{
    internal static partial UsersFilterParameters ToFilterParameters(
        GetUsersListRequest request);

    internal static PaginationResult<UserListItemResponse> ToResponse(
        PaginationResult<UserListItemReadModel> pagedResult)
    {
        return PaginationResult<UserListItemResponse>.Create(
            pagedResult.Items.Select(ToResponse),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount);
    }

    private static partial UserListItemResponse ToResponse(UserListItemReadModel item);
}
