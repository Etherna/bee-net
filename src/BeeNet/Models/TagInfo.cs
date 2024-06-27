// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

using System;

namespace Etherna.BeeNet.Models
{
    public sealed class TagInfo
    {
        // Constructors.
        internal TagInfo(Clients.Response7 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Uid = response.Uid;
            StartedAt = response.StartedAt;
            Split = response.Split;
            Seen = response.Seen;
            Stored = response.Stored;
            Sent = response.Sent;
            Synced = response.Synced;
        }

        internal TagInfo(Clients.Response8 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Uid = response.Uid;
            StartedAt = response.StartedAt;
            Split = response.Split;
            Seen = response.Seen;
            Stored = response.Stored;
            Sent = response.Sent;
            Synced = response.Synced;
        }

        internal TagInfo(Clients.Tags tags)
        {
            ArgumentNullException.ThrowIfNull(tags, nameof(tags));

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
