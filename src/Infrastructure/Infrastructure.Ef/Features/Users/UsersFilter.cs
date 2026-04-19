using Application.Users.Search;

using Infrastructure.Persistence.Ef.Core;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Ef.Features.Users;

internal sealed class UsersFilter : IFilter<Guid, UserDbModel, UsersSearchParams>
{
    public static IQueryable<UserDbModel> Filter(IQueryable<UserDbModel> dbObjects, UsersSearchParams searchParams)
    {
        if (!string.IsNullOrWhiteSpace(searchParams.Role))
        {
            dbObjects = dbObjects.Where(item => EF.Functions.ILike(item.Role, searchParams.Role));
        }
        if (!string.IsNullOrEmpty(searchParams.SearchQuery))
        {
            dbObjects = dbObjects.Where(item => item.Name.Contains(searchParams.SearchQuery) || item.Email.Contains(searchParams.SearchQuery));
        }

        return dbObjects;
    }
}
