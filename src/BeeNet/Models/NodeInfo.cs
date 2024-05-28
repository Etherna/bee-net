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

namespace Etherna.BeeNet.Models
{
    public sealed class NodeInfo
    {
        // Constructors.
        internal NodeInfo(Clients.Response31 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            BeeMode = response.BeeMode switch
            {
                Clients.Response31BeeMode.Dev => InfoBeeMode.Dev,
                Clients.Response31BeeMode.Full => InfoBeeMode.Full,
                Clients.Response31BeeMode.Light => InfoBeeMode.Light,
                _ => throw new InvalidOperationException()
            };
            ChequebookEnabled = response.ChequebookEnabled;
            SwapEnabled = response.SwapEnabled;
        }

        // Methods.
        /// <summary>
        /// Gives back in what mode the Bee client has been started. The modes are mutually exclusive * `light` - light node; does not participate in forwarding or storing chunks * `full` - full node * `dev` - development mode; Bee client for development purposes, blockchain operations are mocked
        /// </summary>
        public InfoBeeMode BeeMode { get; }
        public bool ChequebookEnabled { get; }
        public bool SwapEnabled { get; }
    }
}
