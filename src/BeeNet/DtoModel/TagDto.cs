using System;

namespace Etherna.BeeNet.DtoModel
{
    public class TagDto
    {
        // Constructors.
        public TagDto(Clients.v1_4.DebugApi.Response31 response31)
        {
            if (response31 is null)
                throw new ArgumentNullException(nameof(response31));

            Total = response31.Total;
            Split = response31.Split;
            Seen = response31.Seen;
            Stored = response31.Stored;
            Sent = response31.Sent;
            Synced = response31.Synced;
            Uid = response31.Uid;
            Address = response31.Address;
            StartedAt = response31.StartedAt;
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
