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

namespace Etherna.BeeNet.Models
{
    public sealed class ChainState
    {
        // Constructors.
        internal ChainState(Clients.Response30 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Block = response.Block;
            ChainTip = response.ChainTip;
            CurrentPrice = BzzBalance.FromPlurString(response.CurrentPrice);
            TotalAmount = BzzBalance.FromPlurString(response.TotalAmount);
        }

        // Properties.
        public long Block { get; }
        public int ChainTip { get; }
        public BzzBalance CurrentPrice { get; }
        public BzzBalance TotalAmount { get; }
    }
}
