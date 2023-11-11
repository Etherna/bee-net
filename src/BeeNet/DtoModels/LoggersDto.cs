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

namespace Etherna.BeeNet.DtoModels
{
    public class LoggersDto
    {
        // Constructors.
        internal LoggersDto(Clients.DebugApi.V5_0_0.Loggers loggers)
        {
            if (loggers is null)
                throw new ArgumentNullException(nameof(loggers));

            Id = loggers.Id;
            Logger = loggers.Logger;
            Subsystem = loggers.Subsystem;
            Verbosity = loggers.Verbosity;
        }

        internal LoggersDto(Clients.DebugApi.V5_0_0.Loggers2 loggers2)
        {
            if (loggers2 is null)
                throw new ArgumentNullException(nameof(loggers2));

            Id = loggers2.Id;
            Logger = loggers2.Logger;
            Subsystem = loggers2.Subsystem;
            Verbosity = loggers2.Verbosity;
        }

        // Properties.
        public string Id { get; set; }
        public string Logger { get; set; }
        public string Subsystem { get; set; }
        public string Verbosity { get; set; }
    }
}
