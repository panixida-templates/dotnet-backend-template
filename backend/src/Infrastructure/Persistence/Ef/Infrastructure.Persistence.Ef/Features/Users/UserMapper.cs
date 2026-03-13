using Domain.Users;
using Domain.Users.Enumerations;
using Domain.Users.ValueObjects;

using Infrastructure.Persistence.Ef.Core;

using Riok.Mapperly.Abstractions;

namespace Infrastructure.Persistence.Ef.Features.Users;

[Mapper(IncludedConstructors = MemberVisibility.All)]
internal sealed partial class UserMapper : IEntityMapper<Guid, UserDbModel, User>
{
    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    public static partial void ToDbModel(User entity, UserDbModel dbModel);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial User ToEntity(UserDbModel dbModel);

    private static Guid MapToGuid(UserId source) => source.Value;
    private static UserRole MapRole(string source) => UserRole.FromName(source);
    private static DateOnly MapBirthDate(BirthDate source) => source.Value;
}
