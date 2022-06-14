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

namespace Etherna.BeeNet.DtoModels
{
    public class AuthDto
    {
        // Constructors.
        public AuthDto(Clients.GatewayApi.V2_0_0.Response response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Key = response.Key;
        }

        public AuthDto(Clients.GatewayApi.V3_0_0.Response response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Key = response.Key;
        }

        public AuthDto(Clients.GatewayApi.V3_0_1.Response response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Key = response.Key;
        }

        public AuthDto(Clients.GatewayApi.V3_0_2.Response response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Key = response.Key;
        }

        // Properties.
        public string Key { get; }
    }
}
