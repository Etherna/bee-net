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

namespace Etherna.BeeNet.DtoModels
{
    public class LogDataDto
    {
        // Constructors.
        internal LogDataDto(Clients.DebugApi.V5_0_0.Response44 response44)
        {
            if (response44 is null)
                throw new ArgumentNullException(nameof(response44));
            
            Tree = response44.Tree.ToDictionary(i => i.Key, i => i.Value?.Plus?.ToList() ?? new List<string>());
            Loggers = response44.Loggers.Select(i => new LoggersDto(i)).ToList();
        }

        internal LogDataDto(Clients.DebugApi.V5_0_0.Response45 response45)
        {
            if (response45 is null)
                throw new ArgumentNullException(nameof(response45));

            Tree = response45.Tree.ToDictionary(i => i.Key, i => i.Value?.Plus?.ToList() ?? new List<string>());
            Loggers = response45.Loggers.Select(i => new LoggersDto(i)).ToList();
        }

        // Properties.
        public IDictionary<string, List<string>> Tree { get; set; }
        public ICollection<LoggersDto> Loggers { get; set; }
    }
}
