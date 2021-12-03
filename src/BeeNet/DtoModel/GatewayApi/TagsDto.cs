#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.GatewayApi
{
    public class TagsDto : BaseDto
    {
        public TagsDto(
            int uid, 
            DateTimeOffset startedAt, 
            int total, 
            int processed, 
            int synced, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Uid = uid;
            StartedAt = startedAt;
            Total = total;
            Processed = processed;
            Synced = synced;
        }

        public int Uid { get; set; }

        public DateTimeOffset StartedAt { get; set; }

        public int Total { get; set; }

        public int Processed { get; set; }

        public int Synced { get; set; }
    }
}

#pragma warning restore CA2227
