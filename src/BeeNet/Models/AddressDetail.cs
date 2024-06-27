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
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.Models
{
    public sealed class AddressDetail
    {
        // Constructors.
        internal AddressDetail(Clients.Response20 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Underlay = response.Underlay.Where(i => !string.IsNullOrWhiteSpace(i));
            Overlay = response.Overlay;
            Ethereum = response.Ethereum;
            PublicKey = response.PublicKey;
            PssPublicKey = response.PssPublicKey;
        }
        
        // Properties.
        public string Overlay { get; }
        public IEnumerable<string> Underlay { get; }
        public string Ethereum { get; }
        public string PublicKey { get; }
        public string PssPublicKey { get; }
    }
}
