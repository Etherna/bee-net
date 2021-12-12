using System;

namespace Etherna.BeeNet.DtoModel
{
    public class TagDto
    {
        // Constructors.
        public TagDto(Clients.v1_4.DebugApi.Response31 response)
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
        public int Uid { get; }
        public string Address { get; }
        public DateTimeOffset StartedAt { get; }
    }
}
