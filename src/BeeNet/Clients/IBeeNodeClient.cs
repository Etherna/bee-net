
using System;
using TestAdapter.Clients;
using TestAdapter.Clients.DebugApi;

namespace TestAdapter
{
    public interface IBeeNodeClient
    {
        IFacadeBeeDebugClient DebugClient { get; }
        IFacadeBeeGatewayApiClient GatewayClient { get; }
    }
}