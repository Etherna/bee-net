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
using System.Globalization;

namespace Etherna.BeeNet.Models
{
    public sealed class ResultChequebook
    {
        // Constructors.
        internal ResultChequebook(Clients.Result result)
        {
            ArgumentNullException.ThrowIfNull(result, nameof(result));

            Recipient = result.Recipient;
            LastPayout = long.Parse(result.LastPayout, CultureInfo.InvariantCulture);
            Bounced = result.Bounced;
        }

        // Properties.
        public bool Bounced { get; }
        public long LastPayout { get; }
        public string Recipient { get; }
    }
}
