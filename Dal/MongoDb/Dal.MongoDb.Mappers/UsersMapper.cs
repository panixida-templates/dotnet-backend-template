using Dal.MongoDb.DbModels;
using Dal.MongoDb.Mappers.Core;

using Entities;

using Riok.Mapperly.Abstractions;

namespace Dal.MongoDb.Mappers;

[Mapper]
public sealed partial class UsersMapper : IMapper<Guid, UserDbModel, User>
{
    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    public static partial void ToDbModel(User entity, UserDbModel dbModel);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial User ToEntity(UserDbModel dbModel);
}
