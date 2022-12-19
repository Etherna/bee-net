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
using System.Globalization;

namespace Etherna.BeeNet.DtoModels
{
    public class ChainStateDto
    {
        // Constructors.
        internal ChainStateDto(Clients.DebugApi.V3_2_0.Response14 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Block = response.Block;
            TotalAmount = Convert.ToInt64(response.TotalAmount, CultureInfo.InvariantCulture);
            CurrentPrice = Convert.ToInt64(response.CurrentPrice, CultureInfo.InvariantCulture);
        }

        internal ChainStateDto(Clients.GatewayApi.V3_2_0.Response32 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Block = response.Block;
            TotalAmount = Convert.ToInt64(response.TotalAmount, CultureInfo.InvariantCulture);
            CurrentPrice = Convert.ToInt64(response.CurrentPrice, CultureInfo.InvariantCulture);
        }

        // Properties.
        public long Block { get; }
        public long TotalAmount { get; }
        public long CurrentPrice { get; }
    }
}
