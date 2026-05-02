using Application.Users.GetList;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Ef.Features.Users.Read.Filters;

internal static class UsersFilter
{
    public static IQueryable<UserReadDbModel> Apply(
        IQueryable<UserReadDbModel> dbObjects,
        UsersFilterParameters searchParams)
    {
        if (!string.IsNullOrWhiteSpace(searchParams.Role))
        {
            dbObjects = dbObjects.Where(item => EF.Functions.ILike(item.Role, searchParams.Role));
        }

        return dbObjects;
    }
}
