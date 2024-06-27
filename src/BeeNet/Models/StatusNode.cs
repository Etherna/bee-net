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
