using Application.Users.GetDetails;

using Infrastructure.Ef.Core.Read;

using Riok.Mapperly.Abstractions;

namespace Infrastructure.Ef.Features.Users.Read.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal sealed partial class UserDetailsReadModelMapper : IMapper<Guid, UserReadDbModel, UserDetailsReadModel>
{
    public static partial IQueryable<UserDetailsReadModel> ProjectTo(IQueryable<UserReadDbModel> query);
}