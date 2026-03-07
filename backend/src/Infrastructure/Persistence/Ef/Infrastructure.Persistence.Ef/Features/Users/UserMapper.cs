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

    private static UserRole MapRole(string role) => UserRole.FromName(role);
    private static DateOnly MapBirthDate(BirthDate birthDate) => birthDate.Value;
}
