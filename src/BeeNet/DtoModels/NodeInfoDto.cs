using Etherna.BeeNet.Clients.DebugApi.V1_2_1;
using System;

namespace Etherna.BeeNet.DtoModels
{
    public class NodeInfoDto
    {
        // Constructors.
        public NodeInfoDto(Response14 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BeeMode = response.BeeMode switch
            {
                Response14BeeMode.Dev => BeeModeDto.Dev,
                Response14BeeMode.Full => BeeModeDto.Full,
                Response14BeeMode.Light => BeeModeDto.Light,
                _ => throw new InvalidOperationException()
            };
            ChequebookEnabled = response.ChequebookEnabled;
            GatewayMode = response.GatewayMode;
            SwapEnabled = response.SwapEnabled;
        }

        // Methods.
        /// <summary>
        /// Gives back in what mode the Bee client has been started. The modes are mutually exclusive * `light` - light node; does not participate in forwarding or storing chunks * `full` - full node * `dev` - development mode; Bee client for development purposes, blockchain operations are mocked
        /// </summary>
        public BeeModeDto BeeMode { get; }
        public bool ChequebookEnabled { get; }
        public bool GatewayMode { get; }
        public bool SwapEnabled { get; }
    }
}
