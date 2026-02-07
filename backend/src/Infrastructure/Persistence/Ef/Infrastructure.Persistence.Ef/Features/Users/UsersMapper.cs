using Domain.Entities;

using Infrastructure.Persistence.Ef.Core;

using Riok.Mapperly.Abstractions;

namespace Infrastructure.Persistence.Ef.Features.Users;

[Mapper]
internal sealed partial class UsersMapper : IWriteMapper<Guid, UserDbModel, User>
{
    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    public static partial void ToDbModel(User entity, UserDbModel dbModel);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial User ToEntity(UserDbModel dbModel);
}
