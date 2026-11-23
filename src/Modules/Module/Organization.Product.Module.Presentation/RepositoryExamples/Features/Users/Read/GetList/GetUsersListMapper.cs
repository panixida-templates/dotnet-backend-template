using Organization.Product.Module.Application.RepositoryExamples.Users.Common.Filters;
using Organization.Product.Module.Application.RepositoryExamples.Users.Read.GetList;

using Riok.Mapperly.Abstractions;

namespace Organization.Product.Module.Presentation.RepositoryExamples.Features.Users.Read.GetList;

[Mapper(IncludedConstructors = MemberVisibility.All)]
internal static partial class GetUsersListMapper
{
    internal static partial UsersFilterParameters ToFilterParameters(
        GetUsersListRequest request);

    internal static partial PaginationResult<UserListItemResponse> ToResponse(
        PaginationResult<UserListItemReadModel> pagedResult);
}
