using System;

namespace Etherna.BeeNet.DtoModel
{
    public class TagInfoDto
    {
        public TagInfoDto(
            int uid, 
            DateTimeOffset startedAt,
            int total, 
            int processed,
            int synced)
        {
            Uid = uid;
            StartedAt = startedAt;
            Total = total;
            Processed = processed;
            Synced = synced;
        }

        public int Uid { get; set; } = default!;
        
        public DateTimeOffset StartedAt { get; set; } = default!;
        
        public int Total { get; set; } = default!;
        
        public int Processed { get; set; } = default!;
        
        public int Synced { get; set; } = default!;
    }
}
