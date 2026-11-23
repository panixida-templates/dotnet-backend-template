using Organization.Product.Module.Application.RepositoryExamples.Users.Read.GetList;
using Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Read.DbModels;

using Riok.Mapperly.Abstractions;

namespace Organization.Product.Module.Infrastructure.RepositoryExamples.Persistence.Features.Users.Read.GetList;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal sealed partial class UserListItemReadModelMapper
    : IReadModelMapper<Guid, UserReadDbModel, UserListItemReadModel>
{
    public static partial IQueryable<UserListItemReadModel> ProjectTo(
        IQueryable<UserReadDbModel> query);
}
