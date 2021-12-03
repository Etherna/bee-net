
using Etherna.BeeNet.Clients;

namespace Etherna.BeeNet
{
    public interface IBeeNodeClient
    {
        IBeeNodeDebugClient DebugClient { get; }
        IBeeGatewayApiClient GatewayClient { get; }
    }
}