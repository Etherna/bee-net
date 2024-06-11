// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Etherna.BeeNet.Clients;
using System;

namespace Etherna.BeeNet.Models
{
    public sealed class CheckPinsResult
    {
        internal CheckPinsResult(Response15 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Hash = response.Reference;
            Invalid = response.Invalid;
            Missing = response.Missing;
            Total = response.Total;
        }
        
        public SwarmHash Hash { get; }
        public int Invalid { get; }
        public int Missing { get; }
        public int Total { get; }
    }
}