//   Copyright 2021-present Etherna Sagl
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
using System.Text.Json;

namespace Etherna.BeeNet.DtoModel
{
    public class BatchDto
    {
        // Constructors.
        public BatchDto(Clients.DebugApi.v1_2_0.Response39 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BatchId = ((JsonElement)response.BatchID).ToString();
        }

        public BatchDto(Clients.DebugApi.v1_2_0.Response40 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BatchId = ((JsonElement)response.BatchID).ToString();
        }

        public BatchDto(Clients.DebugApi.v1_2_0.Response41 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BatchId = ((JsonElement)response.BatchID).ToString();
        }

        // Properties.
        public string BatchId { get; }
    }
}
