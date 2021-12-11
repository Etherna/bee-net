using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel
{
    public class BucketDto
    {
        public BucketDto(Clients.v1_4.DebugApi.Buckets buckets)
        {
            if (buckets is null)
            {
                return;
            }

            BucketId = buckets.BucketID;
            Collisions = buckets.Collisions;
        }

        public int BucketId { get; set; } = default!;

        public int Collisions { get; set; } = default!;
    }
}
