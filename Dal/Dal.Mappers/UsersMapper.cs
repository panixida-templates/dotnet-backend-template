using Dal.DbModels;
using Dal.Mappers.Core;

using Entities;

using Riok.Mapperly.Abstractions;

namespace Dal.Mappers;

[Mapper]
public sealed partial class UsersMapper : IMapper<UserDbModel, User>
{
    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    [MapperIgnoreTarget(nameof(UserDbModel.Id))]
    [MapperIgnoreSource(nameof(User.Id))]
    public static partial UserDbModel ToDbModel(User entity);
    public static partial IEnumerable<UserDbModel> ToDbModel(IEnumerable<User> entities);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial User ToEntity(UserDbModel dbModel);
    public static partial IEnumerable<User> ToEntity(IEnumerable<UserDbModel> dbModels);
}
