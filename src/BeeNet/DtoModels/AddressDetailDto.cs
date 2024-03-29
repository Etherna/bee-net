﻿//   Copyright 2021-present Etherna Sagl
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
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModels
{
    public class AddressDetailDto
    {
        // Constructors.
        public AddressDetailDto(Clients.DebugApi.V1_2_0.Response response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Underlay = response.Underlay.Where(i => !string.IsNullOrWhiteSpace(i));
            Overlay = response.Overlay;
            Ethereum = response.Ethereum;
            PublicKey = response.PublicKey;
            PssPublicKey = response.PssPublicKey;
        }

        public AddressDetailDto(Clients.DebugApi.V1_2_1.Response response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Underlay = response.Underlay.Where(i => !string.IsNullOrWhiteSpace(i));
            Overlay = response.Overlay;
            Ethereum = response.Ethereum;
            PublicKey = response.PublicKey;
            PssPublicKey = response.PssPublicKey;
        }
        public AddressDetailDto(Clients.DebugApi.V2_0_0.Response response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

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
