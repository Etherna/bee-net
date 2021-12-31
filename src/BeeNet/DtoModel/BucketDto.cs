using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel
{
    public class BucketDto
    {
        // Constructors.
        public BucketDto(Clients.v1_4_1.DebugApi.Buckets buckets)
        {
            if (buckets is null)
                throw new ArgumentNullException(nameof(buckets));

            BucketId = buckets.BucketID;
            Collisions = buckets.Collisions;
        }


        // Properties.
        public int BucketId { get; }
        public int Collisions { get; }
    }
}
