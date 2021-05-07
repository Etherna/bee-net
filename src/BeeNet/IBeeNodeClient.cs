using Etherna.BeeNet.Clients.DebugApi;
using Etherna.BeeNet.Clients.GatewayApi;
using System;

namespace Etherna.BeeNet
{
    public interface IBeeNodeClient
    {
        IBeeDebugClient? DebugClient { get; }
        IBeeGatewayClient? GatewayClient { get; }
    }
}