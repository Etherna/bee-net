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
        public ChainStateDto(Clients.DebugApi.V1_2_0.Response13 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Block = response.Block;
            TotalAmount = response.TotalAmount.ToString(CultureInfo.InvariantCulture);
            CurrentPrice = response.CurrentPrice.ToString(CultureInfo.InvariantCulture);
        }

        public ChainStateDto(Clients.DebugApi.V1_2_1.Response13 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Block = response.Block;
            TotalAmount = response.TotalAmount.ToString(CultureInfo.InvariantCulture);
            CurrentPrice = response.CurrentPrice.ToString(CultureInfo.InvariantCulture);
        }

        public ChainStateDto(Clients.DebugApi.V2_0_0.Response13 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Block = response.Block;
            TotalAmount = response.TotalAmount.ToString(CultureInfo.InvariantCulture);
            CurrentPrice = response.CurrentPrice.ToString(CultureInfo.InvariantCulture);
        }

        public ChainStateDto(Clients.DebugApi.V2_0_1.Response13 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Block = response.Block;
            TotalAmount = response.TotalAmount;
            CurrentPrice = response.CurrentPrice;
        }

        // Properties.
        public long Block { get; }
        public string TotalAmount { get; }
        public string CurrentPrice { get; }
    }
}
