using System;

namespace Etherna.BeeNet.DtoModel
{
    public class TagInfoDto
    {
        // Constructors.
        public TagInfoDto(Clients.v1_4.GatewayApi.Tags tags)
        {
            if (tags is null)
                throw new ArgumentNullException(nameof(tags));

            Uid = tags.Uid;
            StartedAt = tags.StartedAt;
            Total = tags.Total;
            Processed = tags.Processed;
            Synced = tags.Synced;
        }

        public TagInfoDto(Clients.v1_4.GatewayApi.Response7 response7)
        {
            if (response7 is null)
                throw new ArgumentNullException(nameof(response7));

            Uid = response7.Uid;
            StartedAt = response7.StartedAt;
            Total = response7.Total;
            Processed = response7.Processed;
            Synced = response7.Synced;
        }

        public TagInfoDto(Clients.v1_4.GatewayApi.Response8 response8)
        {
            if (response8 is null)
                throw new ArgumentNullException(nameof(response8));

            Uid = response8.Uid;
            StartedAt = response8.StartedAt;
            Total = response8.Total;
            Processed = response8.Processed;
            Synced = response8.Synced;
        }


        // Properties.
        public int Uid { get; }
        public DateTimeOffset StartedAt { get; }
        public int Total { get; }
        public int Processed { get; }
        public int Synced { get; }
    }
}
