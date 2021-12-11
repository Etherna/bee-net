using System;

namespace Etherna.BeeNet.DtoModel
{
    public class TagDto
    {
        public TagDto(Clients.v1_4.DebugApi.Response31 response31)
        {
            if (response31 == null)
            {
                return;
            }

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
        public int Total { get; set; } = default!;

        public int Split { get; set; } = default!;

        public int Seen { get; set; } = default!;

        public int Stored { get; set; } = default!;
        
        public int Sent { get; set; } = default!;
        
        public int Synced { get; set; } = default!;
        
        public int Uid { get; set; } = default!;
        
        public string Address { get; set; } = default!;
        
        public DateTimeOffset StartedAt { get; set; } = default!;
    }
}
