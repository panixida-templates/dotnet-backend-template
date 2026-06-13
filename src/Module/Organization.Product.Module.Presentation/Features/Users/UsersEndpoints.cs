using Asp.Versioning;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Organization.Product.Module.Presentation.Features.Users;

internal class UsersEndpoints : IEndpointGroup
{
    internal const string ResourceName = "users";
    internal const string IdRoute = "{id:guid}";

    public void Map(IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.NewVersionedApi();

        var v1Group = api.MapGroup($"{EndpointConstants.EndpointPrefix}/{ResourceName}")
            .HasApiVersion(new ApiVersion(1, 0))
            .WithTags(ResourceName);

        EndpointMapper.MapGroupEndpoints<UsersEndpoints>(
            v1Group,
            endpoints.ServiceProvider);
    }
}
