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

namespace Etherna.BeeNet.Models
{
    public sealed class StatusNode
    {
        // Constructors.
        internal StatusNode(Clients.Response65 status)
        {
            ArgumentNullException.ThrowIfNull(status, nameof(status));

            switch (status.BeeMode)
            {
                case Clients.Response65BeeMode.Light:
                    BeeMode = StatusBeeMode.Light;
                    break;
                case Clients.Response65BeeMode.Full:
                    BeeMode = StatusBeeMode.Full;
                    break;
                case Clients.Response65BeeMode.UltraLight:
                    BeeMode = StatusBeeMode.UltraLight;
                    break;
                case Clients.Response65BeeMode.Unknown:
                    BeeMode = StatusBeeMode.Unknown;
                    break;
            }
            BatchCommitment = status.BatchCommitment;
            ConnectedPeers = status.ConnectedPeers;
            NeighborhoodSize = status.NeighborhoodSize;
            Peer = status.Peer;
            Proximity = status.Proximity;
            PullsyncRate = status.PullsyncRate;
            ReserveSize = status.ReserveSize;
            ReserveSizeWithinRadius = (int)status.ReserveSizeWithinRadius;
            RequestFailed = status.RequestFailed;
            StorageRadius = status.StorageRadius;
        }

        internal StatusNode(Clients.Stamps2 status)
        {
            ArgumentNullException.ThrowIfNull(status, nameof(status));

            switch (status.BeeMode)
            {
                case Clients.StampsBeeMode.Light:
                    BeeMode = StatusBeeMode.Light;
                    break;
                case Clients.StampsBeeMode.Full:
                    BeeMode = StatusBeeMode.Full;
                    break;
                case Clients.StampsBeeMode.UltraLight:
                    BeeMode = StatusBeeMode.UltraLight;
                    break;
                case Clients.StampsBeeMode.Unknown:
                    BeeMode = StatusBeeMode.Unknown;
                    break;
            }
            BatchCommitment = status.BatchCommitment;
            ConnectedPeers = status.ConnectedPeers;
            NeighborhoodSize = status.NeighborhoodSize;
            Peer = status.Peer;
            Proximity = status.Proximity;
            PullsyncRate = status.PullsyncRate;
            ReserveSize = status.ReserveSize;
            ReserveSizeWithinRadius = (int)status.ReserveSizeWithinRadius;
            RequestFailed = status.RequestFailed;
            StorageRadius = status.StorageRadius;
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
