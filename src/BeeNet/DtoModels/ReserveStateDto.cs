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
using System.Globalization;

namespace Etherna.BeeNet.DtoModels
{
    public class ReserveStateDto
    {
        // Constructors.
        internal ReserveStateDto(Clients.DebugApi.V5_0_0.Response12 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Commitment = response.Commitment;
            Radius = response.Radius;
            StorageRadius = response.StorageRadius;
        }

        internal ReserveStateDto(Clients.GatewayApi.V5_0_0.Response29 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Commitment = response.Commitment;
            Radius = response.Radius;
            StorageRadius = response.StorageRadius;
        }

        // Properties.
        public long Commitment { get; }
        public int Radius { get; }
        public int StorageRadius { get; }
    }
}
