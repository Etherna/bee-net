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

using Etherna.BeeNet.Clients.DebugApi;
using System;

namespace Etherna.BeeNet.Models
{
    public sealed class StatusNode
    {
        // Constructors.
        internal StatusNode(Response47 response) 
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            switch (response.BeeMode)
            {
                case Response47BeeMode.Light:
                    BeeMode = StatusBeeMode.Light;
                    break;
                case Response47BeeMode.UltraLight:
                    BeeMode = StatusBeeMode.UltraLight;
                    break;
                case Response47BeeMode.Full:
                    BeeMode = StatusBeeMode.Full;
                    break;
                case Response47BeeMode.Unknown:
                    BeeMode = StatusBeeMode.Unknown;
                    break;
            }
            BatchCommitment = response.BatchCommitment;
            ConnectedPeers = response.ConnectedPeers;
            NeighborhoodSize = response.NeighborhoodSize;
            Peer = response.Peer;
            Proximity = response.Proximity;
            PullsyncRate = response.PullsyncRate;
            ReserveSize = response.ReserveSize;
            ReserveSizeWithinRadius = (int)response.ReserveSizeWithinRadius;
            RequestFailed = response.RequestFailed;
            StorageRadius = response.StorageRadius;
        }

        internal StatusNode(Stamps2 stamps)
        {
            ArgumentNullException.ThrowIfNull(stamps, nameof(stamps));

            switch (stamps.BeeMode)
            {
                case StampsBeeMode.Light:
                    BeeMode = StatusBeeMode.Light;
                    break;
                case StampsBeeMode.UltraLight:
                    BeeMode = StatusBeeMode.UltraLight;
                    break;
                case StampsBeeMode.Full:
                    BeeMode = StatusBeeMode.Full;
                    break;
                case StampsBeeMode.Unknown:
                    BeeMode = StatusBeeMode.Unknown;
                    break;
            }
            BatchCommitment = stamps.BatchCommitment;
            ConnectedPeers = stamps.ConnectedPeers;
            NeighborhoodSize = stamps.NeighborhoodSize;
            Peer = stamps.Peer;
            Proximity = stamps.Proximity;
            PullsyncRate = stamps.PullsyncRate;
            ReserveSize = stamps.ReserveSize;
            ReserveSizeWithinRadius = (int)stamps.ReserveSizeWithinRadius;
            RequestFailed = stamps.RequestFailed;
            StorageRadius = stamps.StorageRadius;
        }

        // Properties.
        public StatusBeeMode BeeMode { get; }
        public int BatchCommitment { get; }
        public int ConnectedPeers { get; }
        public int NeighborhoodSize { get; }
        public string Peer { get; }
        public int Proximity { get; }
        public double PullsyncRate { get; }
        public int ReserveSize { get; }
        public int ReserveSizeWithinRadius { get; }
        public bool? RequestFailed { get; }
        public int StorageRadius { get; }
    }
}
