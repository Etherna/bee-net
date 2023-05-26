using Etherna.BeeNet.Clients.DebugApi.V4_0_0;

namespace Etherna.BeeNet.DtoModels
{
    public class StatusNodeDto
    {
        // Constructors.
        public StatusNodeDto(Response48 response48) 
        {
            switch (response48.BeeMode)
            {
                case Response48BeeMode.Light:
                    BeeMode = BeeMode.Light;
                    break;
                case Response48BeeMode.UltraLight:
                    BeeMode = BeeMode.UltraLight;
                    break;
                case Response48BeeMode.Full:
                    BeeMode = BeeMode.Full;
                    break;
                case Response48BeeMode.Unknown:
                    BeeMode = BeeMode.Unknown;
                    break;
            }
            BatchTotalAmount = response48.BatchTotalAmount;
            ConnectedPeers = response48.ConnectedPeers;
            NeighborhoodSize = response48.NeighborhoodSize;
            Peer = response48.Peer;
            Proximity = response48.Proximity;
            PullsyncRate = response48.PullsyncRate;
            ReserveSize = response48.ReserveSize;
            RequestFailed = response48.RequestFailed;
            StorageRadius = response48.StorageRadius;
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
            BatchTotalAmount = stampss.BatchTotalAmount;
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
        public string BatchTotalAmount { get; set; } = default!;
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
