using Application.Users.GetList;

using Infrastructure.Ef.Core.Read;

using Riok.Mapperly.Abstractions;

namespace Infrastructure.Ef.Features.Users.Read.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal sealed partial class UserListItemReadModelMapper : IMapper<Guid, UserReadDbModel, UserListItemReadModel>
{
    public static partial IQueryable<UserListItemReadModel> ProjectTo(IQueryable<UserReadDbModel> query);
}
