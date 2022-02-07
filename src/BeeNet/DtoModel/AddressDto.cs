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

using System;

namespace Etherna.BeeNet.DtoModel
{
    public class AddressDto
    {
        // Constructors.
        public AddressDto(Clients.v1_4_1.DebugApi.Peers? peer)
        {
            if (peer is null)
                throw new ArgumentNullException(nameof(peer));

            Address = peer.Address;
        }

        public AddressDto(Clients.v1_4_1.DebugApi.Peers2? peers)
        {
            if (peers is null)
                throw new ArgumentNullException(nameof(peers));

            Address = peers.Address;
        }

        public AddressDto(string addresses)
        {
            if (addresses is null)
                throw new ArgumentNullException(nameof(addresses));

            Address = addresses;
        }


        // Properties.
        public string Address { get; }
    }
}
