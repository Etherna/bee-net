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
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.Models
{
    public sealed class LogData
    {
        // Constructors.
        internal LogData(Clients.DebugApi.Response45 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Tree = response.Tree.ToDictionary(i => i.Key, i => i.Value?.Plus?.ToList() ?? new List<string>());
            Loggers = response.Loggers.Select(i => new Loggers(i)).ToList();
        }

        internal LogData(Clients.DebugApi.Response46 response)
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
