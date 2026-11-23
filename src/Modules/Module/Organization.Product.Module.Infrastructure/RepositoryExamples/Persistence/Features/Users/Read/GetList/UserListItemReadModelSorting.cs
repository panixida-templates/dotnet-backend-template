using Organization.Product.Module.Application.RepositoryExamples.Users.Read.GetList;

namespace Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Read.GetList;

internal sealed partial class UserListItemReadModelSorting : IReadModelSorting<UserListItemReadModel>
{
    public static SortingParameters DefaultSorting { get; } =
        SortingParameters.Ascending(nameof(UserListItemReadModel.Id));
}
