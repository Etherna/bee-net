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

namespace Etherna.BeeNet.DtoModels
{
    public class StatusNodeDto
    {
        // Constructors.
        public StatusNodeDto(Response46 response46) 
        {
            switch (response46.BeeMode)
            {
                case Response46BeeMode.Light:
                    BeeMode = BeeMode.Light;
                    break;
                case Response46BeeMode.UltraLight:
                    BeeMode = BeeMode.UltraLight;
                    break;
                case Response46BeeMode.Full:
                    BeeMode = BeeMode.Full;
                    break;
                case Response46BeeMode.Unknown:
                    BeeMode = BeeMode.Unknown;
                    break;
            }
            BatchCommitment = response46.BatchCommitment;
            ConnectedPeers = response46.ConnectedPeers;
            NeighborhoodSize = response46.NeighborhoodSize;
            Peer = response46.Peer;
            Proximity = response46.Proximity;
            PullsyncRate = response46.PullsyncRate;
            ReserveSize = response46.ReserveSize;
            RequestFailed = response46.RequestFailed;
            StorageRadius = response46.StorageRadius;
        }

        public StatusNodeDto(Stamps2 stampss)
        {
            switch (stampss.BeeMode)
            {
                case Stamps2BeeMode.Light:
                    BeeMode = BeeMode.Light;
                    break;
                case Stamps2BeeMode.UltraLight:
                    BeeMode = BeeMode.UltraLight;
                    break;
                case Stamps2BeeMode.Full:
                    BeeMode = BeeMode.Full;
                    break;
                case Stamps2BeeMode.Unknown:
                    BeeMode = BeeMode.Unknown;
                    break;
            }
            BatchCommitment = stampss.BatchCommitment;
            ConnectedPeers = stampss.ConnectedPeers;
            NeighborhoodSize = stampss.NeighborhoodSize;
            Peer = stampss.Peer;
            Proximity = stampss.Proximity;
            PullsyncRate = stampss.PullsyncRate;
            ReserveSize = stampss.ReserveSize;
            RequestFailed = stampss.RequestFailed;
            StorageRadius = stampss.StorageRadius;
        }

        // Properties.
        public BeeMode BeeMode { get; set; } = default!;
        public int BatchCommitment { get; set; } = default!;
        public int ConnectedPeers { get; set; } = default!;
        public int NeighborhoodSize { get; set; } = default!;
        public string Peer { get; set; } = default!;
        public int Proximity { get; set; } = default!;
        public double PullsyncRate { get; set; } = default!;
        public int ReserveSize { get; set; } = default!;
        public bool? RequestFailed { get; set; } = default!;
        public int StorageRadius { get; set; } = default!;
    }
}
