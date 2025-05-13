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

namespace Etherna.BeeNet.Models
{
    public sealed class StatusNode(
        StatusBeeMode beeMode,
        int batchCommitment,
        int committedDepth,
        int connectedPeers,
        bool isWarmingUp,
        bool isReachable,
        long lastSyncedBlock,
        int neighborhoodSize,
        string overlay,
        int proximity,
        double pullsyncRate,
        int reserveSize,
        int reserveSizeWithinRadius,
        bool? requestFailed,
        int storageRadius)
    {
        // Properties.
        public StatusBeeMode BeeMode { get; } = beeMode;
        public int BatchCommitment { get; } = batchCommitment;
        public int CommittedDepth { get; } = committedDepth;
        public int ConnectedPeers { get; } = connectedPeers;
        public bool IsReachable { get; } = isReachable;
        public bool IsWarmingUp { get; } = isWarmingUp;
        public long LastSyncedBlock { get; } = lastSyncedBlock;
        public int NeighborhoodSize { get; } = neighborhoodSize;
        public string Overlay { get; } = overlay;
        public int Proximity { get; } = proximity;
        public double PullsyncRate { get; } = pullsyncRate;
        public int ReserveSize { get; } = reserveSize;
        public int ReserveSizeWithinRadius { get; } = reserveSizeWithinRadius;
        public bool? RequestFailed { get; } = requestFailed;
        public int StorageRadius { get; } = storageRadius;
    }
}
