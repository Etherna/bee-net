﻿//   Copyright 2021-present Etherna SA
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

namespace Etherna.BeeNet.Models
{
    public sealed class Loggers
    {
        // Constructors.
        internal Loggers(Clients.DebugApi.Loggers loggers)
        {
            ArgumentNullException.ThrowIfNull(loggers, nameof(loggers));

            Id = loggers.Id;
            Logger = loggers.Logger;
            Subsystem = loggers.Subsystem;
            Verbosity = loggers.Verbosity;
        }

        internal Loggers(Clients.DebugApi.Loggers2 loggers)
        {
            ArgumentNullException.ThrowIfNull(loggers, nameof(loggers));

            Id = loggers.Id;
            Logger = loggers.Logger;
            Subsystem = loggers.Subsystem;
            Verbosity = loggers.Verbosity;
        }

        // Properties.
        public string Id { get; set; }
        public string Logger { get; set; }
        public string Subsystem { get; set; }
        public string Verbosity { get; set; }
    }
}