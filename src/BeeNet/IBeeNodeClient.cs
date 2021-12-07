
using Etherna.BeeNet.Clients;

namespace Etherna.BeeNet
{
    public interface IBeeNodeClient
    {
        IBeeDebugClient? DebugClient { get; }
        IBeeGatewayClient? GatewayClient { get; }
    }
}