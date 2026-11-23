using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;

using PANiXiDA.Core.Presentation.SignalR.Hubs;

namespace Organization.Product.Module.Presentation.RepositoryExamples.Streaming;

public sealed class ExampleHub : Hub, IHubEndpoint
{
    public static HubEndpointConventionBuilder Map(HubMapBuilder hubs)
    {
        return hubs.MapHub<ExampleHub>("/hubs/example");
    }
}
