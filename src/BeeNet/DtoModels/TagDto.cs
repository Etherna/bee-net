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

namespace Etherna.BeeNet.DtoModels
{
    public class TagDto
    {
        // Constructors.
        public TagDto(Clients.DebugApi.V1_2_0.Response31 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Total = response.Total;
            Split = response.Split;
            Seen = response.Seen;
            Stored = response.Stored;
            Sent = response.Sent;
            Synced = response.Synced;
            Uid = response.Uid;
            Address = response.Address;
            StartedAt = response.StartedAt;
        }

        public TagDto(Clients.DebugApi.V1_2_1.Response32 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Total = response.Total;
            Split = response.Split;
            Seen = response.Seen;
            Stored = response.Stored;
            Sent = response.Sent;
            Synced = response.Synced;
            Uid = response.Uid;
            Address = response.Address;
            StartedAt = response.StartedAt;
        }

        // Properties.
        public int Total { get; }
        public int Split { get; }
        public int Seen { get; }
        public int Stored { get; }
        public int Sent { get; }
        public int Synced { get; }
        public int Uid { get; }
        public string Address { get; }
        public DateTimeOffset StartedAt { get; }
    }
}
