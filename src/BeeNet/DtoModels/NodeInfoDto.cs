//   Copyright 2021-present Etherna SA
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

using System;

namespace Etherna.BeeNet.DtoModels
{
    public class NodeInfoDto
    {
        // Constructors.
        internal NodeInfoDto(Clients.DebugApi.Response14 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            BeeMode = response.BeeMode switch
            {
                Clients.DebugApi.Response14BeeMode.Dev => BeeModeDto.Dev,
                Clients.DebugApi.Response14BeeMode.Full => BeeModeDto.Full,
                Clients.DebugApi.Response14BeeMode.Light => BeeModeDto.Light,
                _ => throw new InvalidOperationException()
            };
            ChequebookEnabled = response.ChequebookEnabled;
            SwapEnabled = response.SwapEnabled;
        }

        internal NodeInfoDto(Clients.GatewayApi.Response30 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            BeeMode = response.BeeMode switch
            {
                Clients.GatewayApi.Response30BeeMode.Dev => BeeModeDto.Dev,
                Clients.GatewayApi.Response30BeeMode.Full => BeeModeDto.Full,
                Clients.GatewayApi.Response30BeeMode.Light => BeeModeDto.Light,
                _ => throw new InvalidOperationException()
            };
            ChequebookEnabled = response.ChequebookEnabled;
            SwapEnabled = response.SwapEnabled;
        }

        // Methods.
        /// <summary>
        /// Gives back in what mode the Bee client has been started. The modes are mutually exclusive * `light` - light node; does not participate in forwarding or storing chunks * `full` - full node * `dev` - development mode; Bee client for development purposes, blockchain operations are mocked
        /// </summary>
        public BeeModeDto BeeMode { get; }
        public bool ChequebookEnabled { get; }
        public bool SwapEnabled { get; }
    }
}
