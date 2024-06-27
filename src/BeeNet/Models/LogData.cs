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
    public sealed class LogData
    {
        // Constructors.
        internal LogData(Clients.Response63 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Tree = response.Tree.ToDictionary(i => i.Key, i => i.Value?.Plus?.ToList() ?? new List<string>());
            Loggers = response.Loggers.Select(i => new Loggers(i)).ToList();
        }

        internal LogData(Clients.Response64 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Tree = response.Tree.ToDictionary(i => i.Key, i => i.Value?.Plus?.ToList() ?? new List<string>());
            Loggers = response.Loggers.Select(i => new Loggers(i)).ToList();
        }

        // Properties.
        public IDictionary<string, List<string>> Tree { get; }
        public ICollection<Loggers> Loggers { get; }
    }
}
