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
using System.IO;

namespace Etherna.BeeNet.DtoModels
{
    public class FileResponseDto
    {
        // Constructors.
        internal FileResponseDto(Clients.GatewayApi.V5_0_0.FileResponse response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Stream = response.Stream;
            IsFeed = response.Headers.ContainsKey("Swarm-Feed-Index");
        }

        // Properties.
        public bool IsFeed { get; }
        public Stream Stream { get; }
    }
}