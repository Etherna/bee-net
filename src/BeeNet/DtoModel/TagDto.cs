using System;

namespace Etherna.BeeNet.DtoModel
{
    public class TagDto
    {
        public TagDto(
            int total, 
            int split, 
            int seen, 
            int stored, 
            int sent, 
            int synced, 
            int uid, 
            string address, 
            DateTimeOffset startedAt)
        {
            Total = total;
            Split = split;
            Seen = seen;
            Stored = stored;
            Sent = sent;
            Synced = synced;
            Uid = uid;
            Address = address;
            StartedAt = startedAt;
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
