//   Copyright 2021-present Etherna Sagl
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Etherna.BeeNet.Clients.DebugApi.V1_2_1;
using Etherna.BeeNet.Clients.DebugApi.V2_0_0;
using System;

namespace Etherna.BeeNet.DtoModels
{
    public class NodeInfoDto
    {
        // Constructors.
        public NodeInfoDto(Clients.DebugApi.V1_2_1.Response14 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BeeMode = response.BeeMode switch
            {
                Clients.DebugApi.V1_2_1.Response14BeeMode.Dev => BeeModeDto.Dev,
                Clients.DebugApi.V1_2_1.Response14BeeMode.Full => BeeModeDto.Full,
                Clients.DebugApi.V1_2_1.Response14BeeMode.Light => BeeModeDto.Light,
                _ => throw new InvalidOperationException()
            };
            ChequebookEnabled = response.ChequebookEnabled;
            GatewayMode = response.GatewayMode;
            SwapEnabled = response.SwapEnabled;
        }

        public NodeInfoDto(Clients.DebugApi.V2_0_0.Response14 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BeeMode = response.BeeMode switch
            {
                Clients.DebugApi.V2_0_0.Response14BeeMode.Dev => BeeModeDto.Dev,
                Clients.DebugApi.V2_0_0.Response14BeeMode.Full => BeeModeDto.Full,
                Clients.DebugApi.V2_0_0.Response14BeeMode.Light => BeeModeDto.Light,
                _ => throw new InvalidOperationException()
            };
            ChequebookEnabled = response.ChequebookEnabled;
            GatewayMode = response.GatewayMode;
            SwapEnabled = response.SwapEnabled;
        }

        public NodeInfoDto(Clients.DebugApi.V2_0_1.Response14 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BeeMode = response.BeeMode switch
            {
                Clients.DebugApi.V2_0_1.Response14BeeMode.Dev => BeeModeDto.Dev,
                Clients.DebugApi.V2_0_1.Response14BeeMode.Full => BeeModeDto.Full,
                Clients.DebugApi.V2_0_1.Response14BeeMode.Light => BeeModeDto.Light,
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
