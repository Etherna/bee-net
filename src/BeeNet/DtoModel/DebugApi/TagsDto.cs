#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class TagsDto : BaseDto
    {
        public TagsDto(
            int total, 
            int split, 
            int seen, 
            int stored, 
            int sent, 
            int synced, 
            int uid, 
            string address, 
            DateTimeOffset startedAt, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
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

        public int Total { get; set; }

        public int Split { get; set; }

        public int Seen { get; set; }

        public int Stored { get; set; }

        public int Sent { get; set; }

        public int Synced { get; set; }

        public int Uid { get; set; }

        public string Address { get; set; }

        public DateTimeOffset StartedAt { get; set; }
    }
}

#pragma warning restore CA2227