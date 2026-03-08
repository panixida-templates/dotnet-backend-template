using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Presentation.Http.Features.Users.Create;
using Presentation.Http.Features.Users.Delete;
using Presentation.Http.Features.Users.GetById;
using Presentation.Http.Features.Users.Update;

namespace Presentation.Http.Features.Users;

internal static class UsersEndpoints
{
    internal static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/users")
            .WithTags("Users");

        group.MapCreateUserEndpoint();
        group.MapUpdateUserEndpoint();
        group.MapDeleteUserEndpoint();
        group.MapGetByIdUserEndpoint();
        //group.MapSearchUsersEndpoint();

        return endpoints;
    }
}
