using Asp.Versioning;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Presentation.Features.Users.GetDetails;
using Presentation.Http.Common;
using Presentation.Http.Features.Users.Create;
using Presentation.Http.Features.Users.Delete;
using Presentation.Http.Features.Users.Update;

namespace Presentation.Http.Features.Users;

internal static class UsersEndpoints
{
    internal const string ResourceName = "users";
    internal const string IdRoute = "{id:guid}";

    internal static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var usersApi = endpoints.NewVersionedApi();

        var v1Group = usersApi.MapGroup($"{EndpointConstants.EndpointPrefix}/{ResourceName}")
            .HasApiVersion(new ApiVersion(1, 0))
            .WithTags(ResourceName);

        v1Group.MapCreateUserEndpoint();
        v1Group.MapUpdateUserEndpoint();
        v1Group.MapDeleteUserEndpoint();
        v1Group.MapGetByIdUserEndpoint();
        // v1Group.MapSearchUsersEndpoint();

        return endpoints;
    }
}
