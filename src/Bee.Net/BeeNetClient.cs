using Etherna.BeeNet.Clients.DebugApi;
using Etherna.BeeNet.Clients.GatewayApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet
{
    public class BeeNetClient : IBeeNetClient
    {
        // Fields.
        private readonly int debugApiPort;
        private readonly int gatewayApiPort;

        // Constructors.
        public BeeNetClient(
            string baseUrl = "http://localhost",
            int gatewayApiPort = 1633,
            int debugApiPort = 1635)
        {
            this.debugApiPort = debugApiPort;
            this.gatewayApiPort = gatewayApiPort;

            // Generate api clients.
        }

        // Properties.
        public IBeeDebugClient BeeDebugClient { get; }
        public IBeeGatewayClient BeeGatewayClient { get; }
    }
}
