using System;

namespace Etherna.BeeNet.DtoModel
{
    public class TagInfoDto
    {
        public TagInfoDto(Clients.v1_4.GatewayApi.Tags tags)
        {
            if (tags == null)
            {
                return;
            }

            Uid = tags.Uid;
            StartedAt = tags.StartedAt;
            Total = tags.Total;
            Processed = tags.Processed;
            Synced = tags.Synced;
        }

        public TagInfoDto(Clients.v1_4.GatewayApi.Response7 response7)
        {
            if (response7 == null)
            {
                return;
            }

            Uid = response7.Uid;
            StartedAt = response7.StartedAt;
            Total = response7.Total;
            Processed = response7.Processed;
            Synced = response7.Synced;
        }

        public TagInfoDto(Clients.v1_4.GatewayApi.Response8 response8)
        {
            if (response8 == null)
            {
                return;
            }

            Uid = response8.Uid;
            StartedAt = response8.StartedAt;
            Total = response8.Total;
            Processed = response8.Processed;
            Synced = response8.Synced;
        }
        


        public int Uid { get; set; } = default!;
        
        public DateTimeOffset StartedAt { get; set; } = default!;
        
        public int Total { get; set; } = default!;
        
        public int Processed { get; set; } = default!;
        
        public int Synced { get; set; } = default!;
    }
}
