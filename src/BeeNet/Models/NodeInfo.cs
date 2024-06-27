// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

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
