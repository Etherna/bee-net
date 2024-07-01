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

using Etherna.BeeNet.Clients;
using System;

namespace Etherna.BeeNet.Models
{
    public sealed class ChequebookBalance
    {
        // Constructors.
        internal ChequebookBalance(Response27 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            TotalBalance = BzzBalance.FromPlurString(response.TotalBalance);
            AvailableBalance = BzzBalance.FromPlurString(response.AvailableBalance);
        }

        // Properties.
        public BzzBalance TotalBalance { get; }
        public BzzBalance AvailableBalance { get; }
    }
}
