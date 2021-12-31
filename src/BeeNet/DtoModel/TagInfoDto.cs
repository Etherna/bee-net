using System;

namespace Etherna.BeeNet.DtoModel
{
    public class TagInfoDto
    {
        // Constructors.
        public TagInfoDto(Clients.v1_4_1.GatewayApi.Tags tags)
        {
            if (tags is null)
                throw new ArgumentNullException(nameof(tags));

            Uid = tags.Uid;
            StartedAt = tags.StartedAt;
            Total = tags.Total;
            Processed = tags.Processed;
            Synced = tags.Synced;
        }

        public TagInfoDto(Clients.v1_4_1.GatewayApi.Response7 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Uid = response.Uid;
            StartedAt = response.StartedAt;
            Total = response.Total;
            Processed = response.Processed;
            Synced = response.Synced;
        }

        public TagInfoDto(Clients.v1_4_1.GatewayApi.Response8 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Uid = response.Uid;
            StartedAt = response.StartedAt;
            Total = response.Total;
            Processed = response.Processed;
            Synced = response.Synced;
        }


        // Properties.
        public int Uid { get; }
        public DateTimeOffset StartedAt { get; }
        public int Total { get; }
        public int Processed { get; }
        public int Synced { get; }
    }
}
