
using Etherna.BeeNet.Clients;
using Etherna.BeeNet.DtoModel;

namespace Etherna.BeeNet
{
    public interface IBeeNodeClient
    {
        ClientVersions ClientVersion { get; }
        IBeeDebugClient? DebugClient { get; }
        IBeeGatewayClient? GatewayClient { get; }
    }
}