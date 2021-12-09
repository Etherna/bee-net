
using Etherna.BeeNet.Clients;
using Etherna.BeeNet.DtoModel;

namespace Etherna.BeeNet
{
    public interface IBeeNodeClient
    {
        IBeeDebugClient? DebugClient { get; }
        IBeeGatewayClient? GatewayClient { get; }
        ClientVersionEnum ClientVersion { get; }
    }
}