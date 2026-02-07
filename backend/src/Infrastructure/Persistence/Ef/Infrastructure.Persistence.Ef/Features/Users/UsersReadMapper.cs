using Domain.Entities;

using Infrastructure.Persistence.Ef.Core;

using Riok.Mapperly.Abstractions;

namespace Infrastructure.Persistence.Ef.Features.Users;

[Mapper]
internal sealed partial class UsersReadMapper : IReadMapper<Guid, UserDbModel, User>
{
    public static partial IQueryable<User> ProjectTo(IQueryable<UserDbModel> query);
}
