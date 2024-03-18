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
    public class TagInfoDto
    {
        // Constructors.
        internal TagInfoDto(Clients.GatewayApi.Response7 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Uid = response.Uid;
            StartedAt = response.StartedAt;
            Split = response.Split;
            Seen = response.Seen;
            Stored = response.Stored;
            Sent = response.Sent;
            Synced = response.Synced;
        }

        internal TagInfoDto(Clients.GatewayApi.Response8 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Uid = response.Uid;
            StartedAt = response.StartedAt;
            Split = response.Split;
            Seen = response.Seen;
            Stored = response.Stored;
            Sent = response.Sent;
            Synced = response.Synced;
        }

        internal TagInfoDto(Clients.GatewayApi.Tags tags)
        {
            if (tags is null)
                throw new ArgumentNullException(nameof(tags));

            Uid = tags.Uid;
            StartedAt = tags.StartedAt;
            Split = tags.Split;
            Seen = tags.Seen;
            Stored = tags.Stored;
            Sent = tags.Sent;
            Synced = tags.Synced;
        }

        // Properties.
        public long Uid { get; }
        public DateTimeOffset StartedAt { get; }
        public int Split { get; }
        public int Seen { get; }
        public int Stored { get; }
        public int Sent { get; }
        public int Synced { get; }
    }
}
