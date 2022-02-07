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

namespace Etherna.BeeNet.DtoModel
{
    public class TagInfoDto
    {
        // Constructors.
        public TagInfoDto(Clients.v1_4_1.GatewayApi.Tags tags)
        {
            if (tags is null)
                throw new ArgumentNullException(nameof(tags));

            Uid = tags.Uid;
            StartedAt = tags.StartedAt;
            Total = tags.Total;
            Processed = tags.Processed;
            Synced = tags.Synced;
        }

        public TagInfoDto(Clients.v1_4_1.GatewayApi.Response7 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Uid = response.Uid;
            StartedAt = response.StartedAt;
            Total = response.Total;
            Processed = response.Processed;
            Synced = response.Synced;
        }

        public TagInfoDto(Clients.v1_4_1.GatewayApi.Response8 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Uid = response.Uid;
            StartedAt = response.StartedAt;
            Total = response.Total;
            Processed = response.Processed;
            Synced = response.Synced;
        }


        // Properties.
        public int Uid { get; }
        public DateTimeOffset StartedAt { get; }
        public int Total { get; }
        public int Processed { get; }
        public int Synced { get; }
    }
}
