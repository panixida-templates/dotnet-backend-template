using Organization.Product.Module.Application.Users.GetList;
using Organization.Product.Module.Infrastructure.Features.Users.Read;

using PANiXiDA.Core.Infrastructure.Persistence.Ef.Read;

using Riok.Mapperly.Abstractions;

namespace Infrastructure.Ef.Features.Users.Read.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal sealed partial class UserListItemReadModelMapper
    : IReadModelMapper<Guid, UserReadDbModel, UserListItemReadModel>
{
    public static partial IQueryable<UserListItemReadModel> ProjectTo(
        IQueryable<UserReadDbModel> query);
}
