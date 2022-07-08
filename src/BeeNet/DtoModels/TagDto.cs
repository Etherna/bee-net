using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModels
{
    public class TagDto
    {
        // Constructors.
        public TagDto(Clients.DebugApi.V2_0_1.Response32 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Total = response.Total;
            Split = response.Split;
            Seen = response.Seen;
            Stored = response.Stored;
            Sent = response.Sent;
            Synced = response.Synced;
            Uid = response.Uid;
            Address = response.Address;
            StartedAt = response.StartedAt;
        }

        // Properties.
        public int Total { get; }
        public int Split { get; }
        public int Seen { get; }
        public int Stored { get; }
        public int Sent { get; }
        public int Synced { get; }
        public long Uid { get; }
        public string Address { get; }
        public DateTimeOffset StartedAt { get; }
    }
}
