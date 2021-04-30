using Etherna.BeeNet.Clients.DebugApi;
using Etherna.BeeNet.Clients.GatewayApi;
using System;

namespace Etherna.BeeNet
{
    public interface IBeeNetClient
    {
        IBeeDebugClient BeeDebugClient { get; }
        IBeeGatewayClient BeeGatewayClient { get; }
    }
}